using Microsoft.AspNetCore.Mvc;
using Quiz.Site.Models;
using Quiz.Site.Services;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Web.Common.Security;

namespace Quiz.Site.Components
{
    [ViewComponent(Name = "EditProfile")]
    public class EditProfileViewComponent : ViewComponent
    {
        private readonly IAccountService _accountService;
        private readonly IDataTypeValueService _dataTypeValueService;
        private readonly IMemberService _memberService;


        public EditProfileViewComponent(IAccountService accountService, IDataTypeValueService dataTypeValueService, IMemberService memberService)
        {
            _accountService = accountService;
            _dataTypeValueService = dataTypeValueService;
            _memberService = memberService;
        }

        public IViewComponentResult Invoke(MemberIdentityUser user)
        {
            var member = _memberService.GetByEmail(user.Email);
            var memberModel = _accountService.GetMemberModelFromMember(member);

            var enrichedProfile = _accountService.GetEnrichedProfile(memberModel);

            var model = new EditProfileViewModel();

            //model.FirstName = enrichedProfile.FirstName;
            //model.LastName = enrichedProfile.LastName;
            model.Name = enrichedProfile.Name;
            //model.JobTitle = enrichedProfile.JobTitle;
            //selectedJobTitleArray = new[] { enrichedProfile.JobTitle };
            //model.FavouriteColour = enrichedProfile.FavouriteColour;
            //selectedFavouriteColourArray = new[] { enrichedProfile.FavouriteColour };
            //model.Skills = enrichedProfile.Skills;
            //selectedSkillsArray = enrichedProfile.Skills?.ToArray();
            model.AvatarUrl = enrichedProfile.Avatar?.GetCropUrl(120, 120);
            //model.CurrentGalleryItems = enrichedProfile.Gallery;

            //if (model.CurrentGalleryItems != null && model.CurrentGalleryItems.Any())
            //{
            //    var itemCount = model.CurrentGalleryItems.Count();
            //    List<int> order = new List<int>();
            //    for (var i = 0; i < itemCount; i++)
            //    {
            //        order.Add(i);
            //    }

            //    model.GallerySortOrder = string.Join(",", order);
            //}

            //model.JobTitleOptions =
            //    _dataTypeValueService.GetItemsFromValueListDataType("[Dropdown] Job Titles", selectedJobTitleArray);

            //model.SkillsOptions =
            //    _dataTypeValueService.GetItemsFromValueListDataType("[CheckboxList] Skills", selectedSkillsArray);

            //model.FavouriteColourOptions =
            //    _dataTypeValueService.GetItemsFromValueListDataType("[RadioButtonList] Colours", selectedFavouriteColourArray);

            return View(model);
        }
    }
}
