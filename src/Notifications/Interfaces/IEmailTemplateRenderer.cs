namespace Tawtheef.Notifications.Interfaces;

public interface IEmailTemplateRenderer
{
    Task<string> RenderHtmlAsync<T>(string templateKey, T model);
    Task<string> RenderTextAsync<T>(string templateKey, T model);
}
