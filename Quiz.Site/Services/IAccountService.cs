using Quiz.Site.Models;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Security;
using ContentModels = Umbraco.Cms.Web.Common.PublishedModels;

namespace Quiz.Site.Services
{
    public interface IAccountService
    {
        ProfileViewModel GetEnrichedProfile(ContentModels.Member member);

        ContentModels.Member GetMemberModelFromUser(MemberIdentityUser user);

        ContentModels.Member GetMemberModelFromMember(IMember member);

        IMember GetMemberFromUser(MemberIdentityUser user);

        void UpdateProfile(EditProfileViewModel model, ContentModels.Member memberModel, IMember member);
    }
}
