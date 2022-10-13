using Microsoft.AspNetCore.Mvc.Filters;
using Quiz.Site.Services;
using IHostingEnvironment = Umbraco.Cms.Core.Hosting.IHostingEnvironment;

namespace Quiz.Site.Filters;

public class ValidateCaptchaAttribute : ActionFilterAttribute
{
    public async override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var httpContext = context.HttpContext;
        var services = httpContext.RequestServices;
        var environment = services.GetRequiredService<IHostingEnvironment>();
        if (!environment.IsDebugMode)
        {
            var ipAddress = httpContext.Connection.RemoteIpAddress;
            var remoteIp = ipAddress?.IsIPv4MappedToIPv6 == true ? ipAddress.MapToIPv4().ToString() : ipAddress?.ToString() ?? string.Empty;
            var logger = services.GetRequiredService<ILogger<ValidateCaptchaAttribute>>();
            var captchaService = services.GetRequiredService<IhCaptchaService>();

            try
            {
                var hasValidCaptcha = await captchaService.ValidateAsync(remoteIp, httpContext.Request.Form["h-captcha-response"]);
                if (!hasValidCaptcha)
                {
                    context.ModelState.AddModelError("h-captcha-response", "Invalid Captcha!");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error When Trying To Validate Captcha");
            }
        }

        await next();
    }
}
