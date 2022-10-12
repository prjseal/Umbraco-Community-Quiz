using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Quiz.Site.Models;
using System.Net.Http.Headers;

namespace Quiz.Site.Services;

// ReSharper disable once InconsistentNaming
internal class hCaptchaService : IhCaptchaService
{
    private readonly hCaptchaConfiguration _hCaptchaConfiguration;
    private readonly IHttpClientFactory _clientFactory;

    public hCaptchaService(IOptions<hCaptchaConfiguration> hCaptchaConfiguration, IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
        _hCaptchaConfiguration = hCaptchaConfiguration.Value;
    }

    public async Task<bool> ValidateAsync(string? remoteIp, string responseText)
    {
        var client = _clientFactory.CreateClient();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
        client.DefaultRequestHeaders.Add("Accept", "*/*");

        var parameters = new List<KeyValuePair<string, string>>
        {
            new("response", responseText),
            new("secret", _hCaptchaConfiguration.SecretKey),
        };

        if (!string.IsNullOrEmpty(remoteIp))
        {
            parameters.Add(new KeyValuePair<string, string>("remoteip", remoteIp));
        }

        var request = new HttpRequestMessage(HttpMethod.Post, "https://hcaptcha.com/siteverify")
        {
            Content = new FormUrlEncodedContent(parameters)
        };

        var response = await client.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            var jsonString = response.Content.ReadAsStringAsync();
            jsonString.Wait();

            var result = JsonConvert.DeserializeObject<hCaptchaVerifyResponse>(jsonString.Result);

            if (result.Success)
            {
                return true;
            }
        }

        if (!response.IsSuccessStatusCode)
        {
            return false;
        }

        return false;
    }
}
