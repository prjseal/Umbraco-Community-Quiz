namespace Quiz.Site.Services;

// ReSharper disable once InconsistentNaming
public interface IhCaptchaService
{
    Task<bool> ValidateAsync(string? remoteIp, string responseText);
}
