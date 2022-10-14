using Newtonsoft.Json.Linq;
using Quiz.Site.Models;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
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


        public ProfileViewModel GetEnrichedProfile(ContentModels.Member member)
        {
            if (member == null) return null;

            var profile = new ProfileViewModel
            {
                //FirstName = member.FirstName,
                //LastName = member.LastName,
                Name = member.Name,
                Email = member.Value<string>("email"),
                //JobTitle = member.JobTitle,
                //FavouriteColour = member.FavouriteColour,
                //Skills = member.Skills,
                Avatar = member.Avatar
                //Gallery = member.Gallery
            };

            return profile;
        }

        public IMember GetMemberFromUser(MemberIdentityUser user)
        {
            return user != null ? _memberService.GetByUsername(user.UserName) : null;
        }

        public ContentModels.Member GetMemberModelFromUser(MemberIdentityUser user)
        {
            using (var umbracoContext = _umbracoContextFactory.EnsureUmbracoContext())
            {
                var member = user != null ? _memberService.GetByUsername(user.UserName) : null;
                if (member == null) return null;
                return umbracoContext.UmbracoContext.PublishedSnapshot.Members.Get(member) as ContentModels.Member;
            }
        }

        public ContentModels.Member GetMemberModelFromMember(IMember member)
        {
            using (var umbracoContext = _umbracoContextFactory.EnsureUmbracoContext())
            {
                if (member == null) return null;
                return umbracoContext.UmbracoContext.PublishedSnapshot.Members.Get(member) as ContentModels.Member;
            }
        }

        public void UpdateProfile(EditProfileViewModel model, ContentModels.Member memberModel, IMember member)
        {
            member.Name = model.Name;
            
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

        public void DeleteProfile(DeleteProfileViewModel model, ContentModels.Member memberModel, IMember member)
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
                //_memberService.Delete(member);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when deleting member profile");
            }
        }
    }
}
