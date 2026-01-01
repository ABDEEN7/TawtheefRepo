namespace Tawtheef.Application.Common.Security;

public static class AuthSchemes
{
    public const string Smart = "Smart";
    public const string AzureOidc  = "Azure";
    public const string AzureCookies = "AzureCookies";
    public const string AppCookieScheme = "AppAuthCookie";
    public const string AppCookieName = ".tawtheef.auth";
}
