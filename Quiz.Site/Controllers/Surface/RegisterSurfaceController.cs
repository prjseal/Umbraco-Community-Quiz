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

namespace Quiz.Site.Controllers.Surface
{
    public class RegisterSurfaceController : SurfaceController
    {
        private readonly IMemberSignInManager _memberSignInManager;
        private readonly IMemberManager _memberManager;
        private readonly IMemberService _memberService;
        private readonly IAccountService _accountService;
        private readonly IBadgeService _badgeService;
        private readonly INotificationRepository _notificationRepository;
        private readonly ILogger<RegisterSurfaceController> _logger;
        private readonly IEmailSender _emailSender;
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
            IEventAggregator eventAggregator
            ) : base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
        {
            _memberSignInManager = memberSignInManager ?? throw new ArgumentNullException(nameof(memberSignInManager));
            _memberManager = memberManager ?? throw new ArgumentNullException(nameof(memberManager));
            _memberService = memberService ?? throw new ArgumentNullException(nameof(memberService));
            _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
            _badgeService = badgeService ?? throw new ArgumentNullException(nameof(badgeService));
            _notificationRepository = notificationRepository ?? throw new ArgumentNullException(nameof(notificationRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
            _globalSettings = globalSettings?.Value ?? throw new ArgumentNullException(nameof(globalSettings));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateCaptcha]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {            
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            var existingMember = _memberService.GetByEmail(model.Email);

            if (existingMember != null)
            {
                await SendAlreadyRegisteredEmail(model);
                _logger.LogInformation("Register: Member has already been registered");
            }
            else
            {
                if (!ModelState.IsValid) return RedirectToCurrentUmbracoPage();


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
            }

            TempData["Success"] = true;

            var result = await _memberSignInManager.PasswordSignInAsync(
                model.Email, model.Password, isPersistent: false, lockoutOnFailure: true);
            
            if (result.Succeeded && DateTime.Now.Date < EarlyAdopterThreshold.Date)
            {
                var member = _accountService.GetMemberFromUser(await _memberManager.GetCurrentMemberAsync());
                
                if(member is not null)
                {
                    AssignEarlyAdopterBadge(member);
                }
            }
            
            var profilePage = CurrentPage.AncestorOrSelf<HomePage>().FirstChildOfType(ProfilePage.ModelTypeAlias);

            return RedirectToUmbracoPage(profilePage);
        }

        public async Task<bool> SendAlreadyRegisteredEmail(RegisterViewModel model)
        {
            try
            {
                var body =
                    "Somebody tried to register an account using your email address, but you are already registered.";

                var fromAddress = _globalSettings.Smtp.From;

                var subject = string.Format("Already Registered");
                var message = new EmailMessage(fromAddress, model.Email, subject, body, false);
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
        
        private void AssignEarlyAdopterBadge(IMember member)
        {
            var memberModel = _accountService.GetMemberModelFromMember(member);
            var earlyAdopterBadge = _badgeService.GetBadgeByName("Early Adopter");
            
            if(memberModel is not null && !_badgeService.HasBadge(memberModel, earlyAdopterBadge))
            {
                if(_badgeService.AddBadgeToMember(member, earlyAdopterBadge))
                {
                    _notificationRepository.Create(new Notification()
                    {
                        BadgeId = earlyAdopterBadge.GetUdiObject().ToString(),
                        MemberId = memberModel.Id,
                        Message = "New badge earned - " + earlyAdopterBadge.Name
                    });

                    TempData["ShowToast"] = true;
                }
            }
        }
    }
}
