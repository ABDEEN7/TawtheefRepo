namespace Tawtheef.Notifications.Interfaces;

public interface IEmailTemplateRenderer
{
    Task<string> RenderHtmlAsync<T>(string templateKey, T model, string language = "ar");
    Task<string> RenderTextAsync<T>(string templateKey, T model, string language = "ar");
    
    Task<string> RenderHtmlAsync(string templateKey, string payloadJson, string language = "ar");
    Task<string> RenderTextAsync(string templateKey, string payloadJson, string language = "ar");
    string GetDefaultSubject(string templateKey, string language = "ar");
}
