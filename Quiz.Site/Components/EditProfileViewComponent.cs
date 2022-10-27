using Microsoft.AspNetCore.Mvc;
using Quiz.Site.Models;
using Quiz.Site.Services;
using Umbraco.Cms.Core.Security;

namespace Quiz.Site.Components
{
    [ViewComponent(Name = "EditProfile")]
    public class EditProfileViewComponent : ViewComponent
    {
        private readonly IAccountService _accountService;
        private readonly IDataTypeValueService _dataTypeValueService;

        public EditProfileViewComponent(IAccountService accountService, IDataTypeValueService dataTypeValueService)
        {
            _accountService = accountService;
            _dataTypeValueService = dataTypeValueService;
        }

        public IViewComponentResult Invoke(MemberIdentityUser user)
        {
            var member = _accountService.GetMemberModelFromUser(user);

            var enrichedProfile = _accountService.GetEnrichedProfile(member);

            var model = new EditProfileViewModel();

            model.Name = enrichedProfile.Name;
            model.AvatarUrl = enrichedProfile.Avatar?.GetCropUrl(120, 120);
            model.HideProfile = enrichedProfile.HideProfile;

            return View(model);
        }
    }
}
