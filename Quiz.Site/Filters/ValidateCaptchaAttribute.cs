using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Quiz.Site.Models;
using Quiz.Site.Services;

namespace Quiz.Site.Filters;

public class ValidateCaptchaAttribute : ActionFilterAttribute
{
    public async override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        context.ActionArguments.TryGetValue("model", out object model);

        if (model is RegisterViewModel registerViewModel && !string.IsNullOrEmpty(registerViewModel.CaptchaResponse))
        {
            var captchaSettings = new CaptchaSettings();
            var ipAddress = context.HttpContext.Connection.RemoteIpAddress;
            var remoteIp = ipAddress?.IsIPv4MappedToIPv6 == true ? ipAddress.MapToIPv4().ToString() : ipAddress?.ToString() ?? string.Empty;
            var services = context.HttpContext.RequestServices;
            var logger = services.GetRequiredService<ILogger<ValidateCaptchaAttribute>>();
            var captchaService = services.GetRequiredService<ICaptchaService>();
            var captchaOptions = services.GetRequiredService<IOptionsMonitor<CaptchaSettings>>();
            captchaOptions.OnChange(config => captchaSettings = config);

            try
            {
                captchaSettings = captchaOptions?.CurrentValue!;
                var hasValidCaptcha = await captchaService.VerifyAsync(registerViewModel.CaptchaResponse, remoteIp);
                if (!hasValidCaptcha)
                {
                    context.ModelState.AddModelError(nameof(RegisterViewModel.CaptchaResponse), "Invalid Captcha!");
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
