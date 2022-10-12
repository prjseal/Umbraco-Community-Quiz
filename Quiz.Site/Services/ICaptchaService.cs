namespace Quiz.Site.Services;

public interface ICaptchaService
{
    Task<bool> VerifyAsync(string token, string remoteIp);
}