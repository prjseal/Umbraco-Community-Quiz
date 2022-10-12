using Microsoft.Extensions.Options;
using Quiz.Site.Models;
using System.Text.Json;

namespace Quiz.Site.Services;

public class CaptchaService : ICaptchaService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CaptchaService> _logger;
    private CaptchaSettings _captchaSettings;

    public CaptchaService(HttpClient httpClient, ILogger<CaptchaService> logger, IOptionsMonitor<CaptchaSettings> captchaSettings)
    {
        _httpClient = httpClient;
        _logger = logger;
        _captchaSettings = captchaSettings.CurrentValue;
        captchaSettings.OnChange(config => _captchaSettings = config);
    }

    public async Task<bool> VerifyAsync(string token, string remoteIp)
    {
        try
        {
            var postData = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("secret", _captchaSettings.SecretKey),
                new KeyValuePair<string, string>("response", token),
                new KeyValuePair<string, string>("remoteip", remoteIp)
            };

            var request = await _httpClient.PostAsync(_captchaSettings.EndPoint, new FormUrlEncodedContent(postData));
            using var response = await request.Content.ReadFromJsonAsync<JsonDocument>();
            
            return response!.RootElement.TryGetProperty("success", out var isSuccessful)
                    && isSuccessful.GetBoolean();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error When Trying To Validate Captcha (Remote)");
        }

        return false;
    }
}
