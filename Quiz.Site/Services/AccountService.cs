using Newtonsoft.Json.Linq;
using Quiz.Site.Models;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.PublishedModels;
using ContentModels = Umbraco.Cms.Web.Common.PublishedModels;

namespace Quiz.Site.Services
{
    public class AccountService : IAccountService
    {
        private readonly IMemberGroupService _memberGroupService;
        private readonly IMediaUploadService _mediaUploadService;
        private readonly IMemberService _memberService;
        private readonly IMediaService _mediaService;
        private readonly IQuizResultRepository _quizResultRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IIdKeyMap _IIdKeyMap;
        private readonly IUmbracoContextFactory _umbracoContextFactory;
        private readonly ILogger<AccountService> _logger;

        public AccountService(IMemberGroupService memberGroupService,
            IMediaUploadService mediaUploadService, IMemberService memberService,
            IUmbracoContextFactory umbracoContextFactory, IMediaService mediaService,
            IIdKeyMap IIdKeyMap, IQuizResultRepository quizResultRepository,
            INotificationRepository notificationRepository,
            ILogger<AccountService> logger)
        {
            _memberGroupService = memberGroupService;
            _mediaUploadService = mediaUploadService;
            _memberService = memberService;
            _umbracoContextFactory = umbracoContextFactory;
            _mediaService = mediaService;
            _logger = logger;
            _IIdKeyMap = IIdKeyMap;
            _quizResultRepository = quizResultRepository;
            _notificationRepository = notificationRepository;
        }

        public ProfileViewModel GetEnrichedProfile(IPublishedContent member)
        {
            if (member == null) return null;

            ContentModels.Member memberModel = (ContentModels.Member)member;

            return GetEnrichedProfile(memberModel);
        }

        public ProfileViewModel GetEnrichedProfile(ContentModels.Member member)
        {
            if (member == null) return null;

            var profile = new ProfileViewModel
            {
                Name = member.Name,
                Email = member.Value<string>("email"),
                Badges = member.Badges?.Select(x => (BadgePage)x) ?? Enumerable.Empty<BadgePage>(),
                Avatar = member.Avatar,
                HideProfile = member.HideProfile
            };

            return profile;
        }

        public IMember GetMemberFromUser(MemberIdentityUser user)
        {
            return user != null ? _memberService.GetByUsername(user.UserName) : null;
        }

        public ContentModels.Member GetMemberModelFromMember(IMember member)
        {
            using (var umbracoContext = _umbracoContextFactory.EnsureUmbracoContext())
            {
                if (member == null) return null;
                return umbracoContext.UmbracoContext.PublishedSnapshot.Members.Get(member) as ContentModels.Member;
            }
        }



        public ContentModels.Member GetMemberModelFromId(int memberId)
        {
            var member = _memberService.GetById(memberId);
            return GetMemberModelFromMember(member);
        }

        public void UpdateProfile(EditProfileViewModel model, IMember member)
        {
            member.Name = model.Name;
            member.SetValue("hideProfile", model.HideProfile);
            
            try
            {
                if (model.Avatar != null)
                {
                    var avatarUdi = _mediaUploadService.CreateMediaItemFromFileUpload(model.Avatar, Guid.Parse("88614415-784f-4421-84c1-5318b75cf2f4"), "Image");

                    if (avatarUdi == null)
                    {
                        _logger.LogError("Avatar UDI is null");
                    }

                    member.SetValue("avatar", avatarUdi);
                }
                else
                {
                    _logger.LogError("Avatar is null");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when updating member profile");
            }

            _memberService.Save(member);
        }

        public void DeleteProfile(DeleteProfileViewModel model, IMember member)
        {
            try
            {
                //remove the profile image first
                var avatar = member.GetValue<GuidUdi>("avatar");

                if (avatar != null)
                {
                    var avatarId = _IIdKeyMap.GetIdForUdi(member.GetValue<GuidUdi>("avatar")).Result;
                    var mediaItem = _mediaService.GetById(avatarId);
                    //if not default item
                    if(mediaItem != null)
                    {
                        _mediaService.Delete(mediaItem);
                    }
                }

                //remove the quiz results
                var quizes = _quizResultRepository.GetAllByMemberId(member.Id);
                
                if(quizes.Any())
                {
                    foreach (var quiz in quizes)
                    {
                        _quizResultRepository.Delete(quiz.Id);
                    }
                }

                //remove the notifications
                var notifications = _notificationRepository.GetAllByMemberId(member.Id);

                if (notifications.Any())
                {
                    foreach(var notif in notifications)
                    {
                        _notificationRepository.Delete(notif.Id);
                    }
                }

                //finally delete the member completely
                _memberService.Delete(member);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when deleting member profile");
            }
        }
    }
}
