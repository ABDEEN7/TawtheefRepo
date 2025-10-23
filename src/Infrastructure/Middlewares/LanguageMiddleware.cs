using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Tawtheef.Infrastructure.Middlewares;

public class LanguageMiddleware(RequestDelegate next)
{
    private const string DefaultLanguage = "ar";

    public async Task Invoke(HttpContext context)
    {
        var language = GetLanguageFromRequest(context);
        
        // Set the language in the context items for use in controllers/services
        context.Items["Language"] = language;
        
        // Set thread culture if needed
        SetCurrentThreadCulture(language);

        await next(context);
    }

    private static string GetLanguageFromRequest(HttpContext context)
    {
        // Priority order: query string -> header -> cookie -> default
        if (context.Request.Query.TryGetValue("lang", out var queryLang))
            return queryLang.ToString().ToLower();

        var acceptLanguage = context.Request.GetTypedHeaders().AcceptLanguage;
        if (acceptLanguage.Count > 0)
            return acceptLanguage[0].Value.ToString().ToLower();

        return context.Request.Cookies.TryGetValue("lang", out var cookieLang) ? cookieLang.ToLower() : DefaultLanguage;
    }

    private static void SetCurrentThreadCulture(string language)
    {
        try
        {
            var culture = new CultureInfo(language);
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
        }
        catch (CultureNotFoundException)
        {
            // Fallback to default if invalid culture
            var culture = new CultureInfo(DefaultLanguage);
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
        }
    }
}
