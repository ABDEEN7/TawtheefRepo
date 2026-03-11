namespace Tawtheef.Notifications.Interfaces;

public interface IEmailTemplateRenderer
{
    Task<string> RenderHtmlAsync<T>(string templateKey, T model);
    Task<string> RenderTextAsync<T>(string templateKey, T model);
    
    Task<string> RenderHtmlAsync(string templateKey, string payloadJson);
    Task<string> RenderTextAsync(string templateKey, string payloadJson);
}
