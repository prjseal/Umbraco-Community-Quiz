using Microsoft.AspNetCore.Mvc;
using Quiz.Site.Models;
using Quiz.Site.Services;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;

[ViewComponent(Name = "DeleteProfile")]
public class DeleteProfileViewComponent : ViewComponent
{
    private readonly IAccountService _accountService;
    private readonly IDataTypeValueService _dataTypeValueService;
    private readonly IMemberService _memberService;

    public DeleteProfileViewComponent(IAccountService accountService, IDataTypeValueService dataTypeValueService,
        IMemberService memberService)
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

        var model = new DeleteProfileViewModel();
        
        model.Name = enrichedProfile.Name;
        model.AvatarUrl = enrichedProfile.Avatar?.GetCropUrl(120, 120);

        return View(model);
    }
}
