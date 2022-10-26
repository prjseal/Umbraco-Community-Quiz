namespace Quiz.Site.Middleware;

public sealed class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;
    public SecurityHeadersMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    public Task Invoke(HttpContext context)
    {
        context.Response.Headers.Add("referrer-policy", "no-referrer");
        context.Response.Headers.Add("x-content-type-options", "nosniff");
        context.Response.Headers.Add("x-frame-options", "DENY");
        context.Response.Headers.Add("X-Permitted-Cross-Domain-Policies", "none");
        context.Response.Headers.Add("x-xss-protection", "1; mode=block");
        context.Response.Headers.Add("cross-origin-opener-policy", "same-origin");
        context.Response.Headers.Add("Permissions-Policy","accelerometer=(),autoplay=(),camera=(),display-capture=(),document-domain=(),encrypted-media=(),fullscreen=(),geolocation=(),gyroscope=(),magnetometer=(),microphone=(),midi=(),payment=(),picture-in-picture=(),publickey-credentials-get=(),screen-wake-lock=(),sync-xhr=(self),usb=(),web-share=(),xr-spatial-tracking=()");
        /*context.Response.Headers.Add("Content-Security-Policy", "");*/

        return _next(context);
    }
}
