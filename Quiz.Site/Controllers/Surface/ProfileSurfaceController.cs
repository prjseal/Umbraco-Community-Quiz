using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Quiz.Site.Enums;
using Quiz.Site.Extensions;
using Quiz.Site.Models;
using Quiz.Site.Notifications;
using Quiz.Site.Notifications.Profile;
using Quiz.Site.Services;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Mail;
using Umbraco.Cms.Core.Models.Email;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Common.PublishedModels;
using Umbraco.Cms.Web.Common.Security;
using Umbraco.Cms.Web.Website.Controllers;

namespace Quiz.Site.Controllers.Surface
{
    public class ProfileSurfaceController : SurfaceController
    {
        private readonly IMemberManager _memberManager;
        private readonly IMemberService _memberService;
        private readonly IMemberSignInManager _memberSignInManager;
        private readonly ILogger<ProfileSurfaceController> _logger;
        private readonly IEmailSender _emailSender;
        private readonly GlobalSettings _globalSettings;
        private readonly IAccountService _accountService;
        private readonly IBadgeService _badgeService;
        private readonly INotificationRepository _notificationRepository;
        private readonly IEventAggregator _eventAggregator;
        private readonly IMemoryCache _memoryCache;

        public ProfileSurfaceController(
            //these are required by the base controller
            IUmbracoContextAccessor umbracoContextAccessor,
            IUmbracoDatabaseFactory databaseFactory,
            ServiceContext services,
            AppCaches appCaches,
            IProfilingLogger profilingLogger,
            IPublishedUrlProvider publishedUrlProvider,
            //these are dependencies we've added
            IMemberManager memberManager,
            IMemberService memberService,
            IMemberSignInManager memberSignInManager,
            ILogger<ProfileSurfaceController> logger,
            IEmailSender emailSender,
            IOptions<GlobalSettings> globalSettings,
            IAccountService accountService,
            IBadgeService badgeService,
            INotificationRepository notificationRepository,
            IEventAggregator eventAggregator,
            IMemoryCache memoryCache
            ) : base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
        {
            _memberManager = memberManager ?? throw new ArgumentNullException(nameof(memberManager));
            _memberSignInManager = memberSignInManager ?? throw new ArgumentNullException(nameof(memberSignInManager));
            _memberService = memberService ?? throw new ArgumentNullException(nameof(memberService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
            _globalSettings = globalSettings?.Value ?? throw new ArgumentNullException(nameof(globalSettings));
            _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
            _badgeService = badgeService ?? throw new ArgumentNullException(nameof(badgeService));
            _notificationRepository = notificationRepository ?? throw new ArgumentNullException(nameof(notificationRepository));
            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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


                var fullName = $"{model.Name} {model.Email}";

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
            }

            TempData["Success"] = true;
            return RedirectToCurrentUmbracoPage();
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

        public async Task<IActionResult> EditProfile(EditProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Edit Profile Model State Invalid");
                var invalidFields = ModelState.ToErrorDictionary();
                if(invalidFields != null && invalidFields.Any())
                {
                    foreach(var item in invalidFields)
                    {
                        _logger.LogError($" - {item.Key}: {item.Value}");
                    }
                }
                await _eventAggregator.PublishAsync(new ProfileUpdatingFailedNotification("Edit Profile Model State Invalid"));
                return CurrentUmbracoPage();
            }
            
            var member = _accountService.GetMemberFromUser(await _memberManager.GetCurrentMemberAsync());

            if (member is null)
            {
                await _eventAggregator.PublishAsync(new ProfileUpdatingFailedNotification("Member is null"));
                _logger.LogError("Member is null");
                return RedirectToCurrentUmbracoPage();
            }

            var memberModel = _accountService.GetMemberModelFromMember(member);

            if (memberModel is null)
            {
                await _eventAggregator.PublishAsync(new ProfileUpdatingFailedNotification("MemberModel is null"));
                _logger.LogError("MemberModel is null");
                return RedirectToCurrentUmbracoPage();
            }

            _logger.LogInformation("Member Model is Not Null");

            var enrichedProfile = _accountService.GetEnrichedProfile(memberModel);
            
            _accountService.UpdateProfile(model, member);

            var badges = enrichedProfile?.Badges ?? Enumerable.Empty<BadgePage>();

            await _eventAggregator.PublishAsync(new ProfileUpdatedNotification(member, enrichedProfile, badges));

            if (_memoryCache.TryGetValue(CacheKey.LeaderBoard, out _))
            {
                _memoryCache.Remove(CacheKey.LeaderBoard);
            }

            return RedirectToCurrentUmbracoPage();
        }

        public async Task<IActionResult> DeleteProfile(DeleteProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Delete Profile Model State Invalid");
                var invalidFields = ModelState.ToErrorDictionary();
                if (invalidFields != null && invalidFields.Any())
                {
                    foreach (var item in invalidFields)
                    {
                        _logger.LogError($" - {item.Key}: {item.Value}");
                    }
                }
                return RedirectToCurrentUmbracoPage();
            }

            var user = await _memberManager.GetCurrentMemberAsync();

            var member = _accountService.GetMemberFromUser(await _memberManager.GetCurrentMemberAsync());

            if (member == null)
            {
                _logger.LogError("Member is null");
                return RedirectToCurrentUmbracoPage();
            }

            //delete the physical account 
            _accountService.DeleteProfile(model, member);
            //sign them out as well
            await _memberSignInManager.SignOutAsync();


            var homePage = CurrentPage.Root();

            return RedirectToUmbracoPage(homePage.Key);
        }
    }
}
