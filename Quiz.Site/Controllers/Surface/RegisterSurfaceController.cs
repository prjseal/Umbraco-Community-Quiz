using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Quiz.Site.Filters;
using Quiz.Site.Extensions;
using Quiz.Site.Models;
using Quiz.Site.Notifications;
using Quiz.Site.Services;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Mail;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.Email;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Common.PublishedModels;
using Umbraco.Cms.Web.Common.Security;
using Umbraco.Cms.Web.Website.Controllers;
using Notification = Quiz.Site.Models.Notification;
using Quiz.Site.Models.EmailViewModels;
using Quiz.Site.Notifications.Member;

namespace Quiz.Site.Controllers.Surface
{
    public class RegisterSurfaceController : SurfaceController
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
        private readonly INotificationRepository _notificationRepository;
        private readonly ILogger<RegisterSurfaceController> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IOptions<GlobalSettings> globalSettings;
        private readonly IEmailBodyService _emailBodyService;
        private readonly GlobalSettings _globalSettings;
        private readonly IEventAggregator _eventAggregator;

        public readonly static DateTime EarlyAdopterThreshold = new DateTime(2022, 11, 5);

        public RegisterSurfaceController(
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
            INotificationRepository notificationRepository,
            ILogger<RegisterSurfaceController> logger,
            IEmailSender emailSender,
            IOptions<GlobalSettings> globalSettings,
            IEmailBodyService emailBodyService,
            IEventAggregator eventAggregator
            ) : base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
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
            _notificationRepository = notificationRepository ?? throw new ArgumentNullException(nameof(notificationRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
            _emailBodyService = emailBodyService;
            this.globalSettings = globalSettings;
            _globalSettings = globalSettings?.Value ?? throw new ArgumentNullException(nameof(globalSettings));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateCaptcha]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await _eventAggregator.PublishAsync(new MemberRegisteringFailedNotification("ModelState Invalid"));
                return CurrentUmbracoPage();
            }

            var existingMember = _memberService.GetByEmail(model.Email);

            if (existingMember != null)
            {
                await _eventAggregator.PublishAsync(new MemberRegisteringFailedNotification("Member has already been registered"));
                await SendAlreadyRegisteredEmail(model);
                _logger.LogInformation("Register: Member has already been registered");
                ModelState.AddModelError("General", "There was an issue registering your account.");
            }

            if (!ModelState.IsValid) return RedirectToCurrentUmbracoPage();

            var hasCreatedMember = await CreateMember(model);

            if(!hasCreatedMember)
            {
                await _eventAggregator.PublishAsync(new MemberRegisteringFailedNotification("An issue occured registering the member"));
                ModelState.AddModelError("General", "There was an issue registering your account.");
                return RedirectToCurrentUmbracoPage();
            }

            TempData["RegisterSuccess"] = true;

            var result = await _memberSignInManager.PasswordSignInAsync(
                model.Email, model.Password, isPersistent: false, lockoutOnFailure: true);
            
            var profilePage = CurrentPage.AncestorOrSelf<HomePage>().FirstChildOfType(ProfilePage.ModelTypeAlias);

            return RedirectToUmbracoPage(profilePage);
        }

        private async Task<bool> CreateMember(RegisterViewModel model)
        {
            try
            {
                var fullName = $"{model.Name}";

                var memberTypeAlias = CurrentPage.HasValue("memberType")
                    ? CurrentPage.Value<string>("memberType")
                    : Constants.Security.DefaultMemberTypeAlias;

                var identityUser = MemberIdentityUser.CreateNew(model.Email, model.Email, memberTypeAlias, isApproved: true, fullName);
                IdentityResult identityResult = await _memberManager.CreateAsync(
                    identityUser,
                    model.Password);

                var member = _memberService.GetByEmail(identityUser.Email);

                _logger.LogInformation("Register: Member created successfully");

                member.Name = model.Name;
                member.IsApproved = true;

                _memberService.Save(member);

                _memberService.AssignRoles(new[] { member.Username }, new[] { "Member" });
                
                await _eventAggregator.PublishAsync(new MemberRegisteredNotification(member));

                return true;
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Unable to create member.");                
            }

            return false;
            
        }

        public async Task<bool> SendAlreadyRegisteredEmail(RegisterViewModel model)
        {
            try
            {
                var body = await _emailBodyService.RenderRazorEmail(new AlreadyRegisteredNotificationModel(){
                    SiteDomain = $"{Request.Scheme}://{Request.Host}"
                });

                var fromAddress = _globalSettings.Smtp.From;

                var subject = string.Format("Already Registered");
                var message = new EmailMessage(fromAddress, model.Email, subject, body, true);
                await _emailSender.SendAsync(message, emailType: "AlreadyRegistered");

                _logger.LogInformation("Already Registered Email Sent Successfully");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error When Trying To Send Already Registered Email");
                return false;
            }
        }
    }
}
