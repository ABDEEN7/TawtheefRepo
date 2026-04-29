using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Razor;

namespace Tawtheef.Notifications.Templates;

public static class RazorHelpers
{
    public static IHtmlContent Raw(string value) => new HtmlString(value);
}
