namespace Quiz.Site.Services;

// ReSharper disable once InconsistentNaming
public interface IhCaptchaService
{
    bool Validate(string? remoteIp, string responseText);
}
