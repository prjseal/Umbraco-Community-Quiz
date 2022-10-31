using Microsoft.AspNetCore.Mvc;
using Quiz.Site.Extensions;
using Quiz.Site.Filters;
using Quiz.Site.Models;
using Quiz.Site.Notifications;
using Quiz.Site.Notifications.Member;
using Quiz.Site.Services;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Common.PublishedModels;
using Umbraco.Cms.Web.Common.Security;
using Umbraco.Cms.Web.Website.Controllers;
using Notification = Quiz.Site.Models.Notification;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;
using Umbraco.Cms.Core.Mail;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Models.Email;
using Quiz.Site.Models.EmailViewModels;
using System.Web;

namespace Quiz.Site.Controllers.Surface
{
    public class AuthSurfaceController : SurfaceController
    {
        private readonly IUmbracoContextAccessor umbracoContextAccessor;
        private readonly IUmbracoDatabaseFactory databaseFactory;
        private readonly ServiceContext services;
        private readonly AppCaches appCaches;
        private readonly IProfilingLogger profilingLogger;
        private readonly IPublishedUrlProvider publishedUrlProvider;
        private readonly IMemberSignInManager _memberSignInManager;
        private readonly IMemberManager _memberManager;
        private readonly IMemberService _memberService;
        private readonly IAccountService _accountService;
        private readonly IBadgeService _badgeService;
        private readonly IEmailSender _emailSender;
        private readonly GlobalSettings _globalSettings;
        private readonly IEmailBodyService _emailBodyService;
        private readonly INotificationRepository _notificationRepository;
        private readonly ILogger<AuthSurfaceController> _logger;
        private readonly IEventAggregator _eventAggregator;
        
        public AuthSurfaceController(
            //these are required by the base controller
            IUmbracoContextAccessor umbracoContextAccessor,
            IUmbracoDatabaseFactory databaseFactory,
            ServiceContext services,
            AppCaches appCaches,
            IProfilingLogger profilingLogger,
            IPublishedUrlProvider publishedUrlProvider,
            //these are dependencies we've added
            IMemberSignInManager memberSignInManager,
            IMemberManager memberManager,
            IMemberService memberService,
            IAccountService accountService,
            IBadgeService badgeService,
            IEmailSender emailSender,
            IOptions<GlobalSettings> globalSettings,
            IEmailBodyService emailBodyService,
            INotificationRepository notificationRepository,
            IEventAggregator eventAggregator,
            ILogger<AuthSurfaceController> logger) : base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
        {
            this.umbracoContextAccessor = umbracoContextAccessor;
            this.databaseFactory = databaseFactory;
            this.services = services;
            this.appCaches = appCaches;
            this.profilingLogger = profilingLogger;
            this.publishedUrlProvider = publishedUrlProvider;
            _memberSignInManager = memberSignInManager ?? throw new ArgumentNullException(nameof(memberSignInManager));
            _memberManager = memberManager ?? throw new ArgumentNullException(nameof(memberManager));
            _memberService = memberService ?? throw new ArgumentNullException(nameof(memberService));
            _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
            _badgeService = badgeService ?? throw new ArgumentNullException(nameof(badgeService));
            this._emailSender = emailSender;
            _globalSettings = globalSettings?.Value ?? throw new ArgumentNullException(nameof(globalSettings));
            this._emailBodyService = emailBodyService;
            _notificationRepository = notificationRepository ?? throw new ArgumentNullException(nameof(notificationRepository));
            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost]
        [ValidateCaptcha]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            SignInResult result = await _memberSignInManager.PasswordSignInAsync(
                model.Username, model.Password, isPersistent: model.RememberMe, lockoutOnFailure: true);

            if (!result.Succeeded)
            {
                await _eventAggregator.PublishAsync(new MemberLoggingInFailedNotification($"Member login unsuccessful for member {model.Username}"));
            }
            else
            {
                var member = _memberService.GetByUsername(model.Username);
                var memberModel = _accountService.GetMemberModelFromMember(member);
                var enrichedProfile = _accountService.GetEnrichedProfile(memberModel);
                var badges = enrichedProfile?.Badges ?? Enumerable.Empty<BadgePage>();

                await _eventAggregator.PublishAsync(new MemberLoggedInNotification(member, badges));
            }

            var profilePage = CurrentPage.AncestorOrSelf<HomePage>().FirstChildOfType(ProfilePage.ModelTypeAlias);

            return RedirectToUmbracoPage(profilePage);
        }

        [HttpPost]
        public async Task<IActionResult> ForgottenPasswordRequest(ForgottenPasswordRequestViewModel model)
        {
            if(!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            await ForgottenPasswordInternal(model.Email);

            TempData.Add("ForgottenPasswordRequested", true);
            return RedirectToCurrentUmbracoPage();
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _memberSignInManager.SignOutAsync();

            return RedirectToCurrentUmbracoUrl();
        }

        private async Task ForgottenPasswordInternal(string email)
        {
            var existingMember = _memberService.GetByEmail(email);
            if(existingMember == null) return;

            var token = TokenService.GenerateToken(DateTime.Now, Quiz.TokenReasons.ForgottenPasswordRequest, new SimpleUserModel{
                Id = existingMember.Id,
                Key = existingMember.Key,
                SecurityStamp = UpdateUsersSecurityStamp(existingMember)
            });

            await SendForgottenPasswordEmail(token, email, $"{CurrentPage.Url(mode: UrlMode.Absolute)}?u={HttpUtility.UrlEncode(existingMember.Email)}");

        }

        private async Task SendForgottenPasswordEmail(string token, string email, string url)
        {
            try
            {
                var body = await _emailBodyService.RenderRazorEmail(new ForgottenPasswordRequestEmailModel(){
                    Token = token,
                    ForgottenPasswordPageUrl = url
                });

                var fromAddress = _globalSettings.Smtp.From;

                var subject = string.Format("Password reset request");
                var message = new EmailMessage(fromAddress, email, subject, body, true);
                await _emailSender.SendAsync(message, emailType: "ForgottenPasswordRequest");

                _logger.LogInformation("Forgotten Password Request Email Sent Successfully");
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error When Trying To Send Forgotten Password Request Email");
                return;
            }
        }

        private string UpdateUsersSecurityStamp(IMember member)
        {
            var securityStamp = Guid.NewGuid().ToString("N");

            member.SecurityStamp = securityStamp;
            
            _memberService.Save(member);
            return securityStamp;
        }
        
    }
}
