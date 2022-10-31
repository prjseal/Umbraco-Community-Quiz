using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Quiz.Site.Models;
using Quiz.Site.Services;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace Quiz.Site.Controllers
{
    public class ForgottenPasswordPageController : RenderController
    {
        private readonly IMemberService memberService;

        public ForgottenPasswordPageController(IMemberService memberService, ILogger<RenderController> logger, ICompositeViewEngine compositeViewEngine, IUmbracoContextAccessor umbracoContextAccessor) 
            : base(logger, compositeViewEngine, umbracoContextAccessor)
        {
            this.memberService = memberService;
        }

        public override IActionResult Index()
        {
            return base.Index();
        }

        [HttpGet]
        public IActionResult Index([FromQuery(Name = "t")] string token, [FromQuery(Name = "u")]string user)
        {
            if(CurrentPage is ForgottenPasswordPage forgottenPasswordPage)
            {
                if(!token.IsNullOrWhiteSpace())
                {
                    forgottenPasswordPage.CurrentToken = token;
                    forgottenPasswordPage.EmailAddress = user;
                    SimpleUserModel userModel = FindUser(user);

                    var tokenValidationResult = TokenService.ValidateToken(
                        Quiz.TokenReasons.ForgottenPasswordRequest,
                        userModel, token, 1);

                    forgottenPasswordPage.ShowPasswordResetForm = tokenValidationResult.Validated;
                    
                    if(!tokenValidationResult.Validated)
                        ViewData.Add("TokenValidationErrors", tokenValidationResult);
                }
                else
                    forgottenPasswordPage.ShowPasswordResetForm = false;

                return CurrentTemplate(forgottenPasswordPage);
            }

            return CurrentTemplate(CurrentPage);
        }

        private SimpleUserModel FindUser(string userEmail)
        {
            if(string.IsNullOrWhiteSpace(userEmail)) return null;

            var member = memberService.GetByEmail(userEmail);
            if(member == null) return null;

            return new SimpleUserModel{
                Id = member.Id,
                Key = member.Key,
                SecurityStamp = member.SecurityStamp
            };
        }
    }
}