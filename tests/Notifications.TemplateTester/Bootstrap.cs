using System.Reflection;
using Microsoft.Extensions.Options;
using Tawtheef.Application.Common.Interfaces.Logging;
using Tawtheef.Application.Common.Interfaces.NotificationServices;
using Tawtheef.Domain.Configurations.Settings;
using Tawtheef.Notifications.Attributes;
using Tawtheef.Notifications.Context;
using Tawtheef.Notifications.Services;
using Tawtheef.Infrastructure.Services.HttpClients;
using Tawtheef.Infrastructure.Services.NotificationServices;

namespace Tawtheef.Notifications.TemplateTester;

internal static class Bootstrap
{
    public static List<TemplateEntry> LoadTemplates(Assembly asm)
    {
        return asm.GetTypes()
            .Where(type => type is { IsAbstract: false, IsInterface: false })
            .Select(type => new TemplateEntry(type, type.GetCustomAttribute<NotificationTemplateAttribute>()!))
            .Where(entry => entry.Attribute != null)
            .Select(entry => entry with { Attribute = entry.Attribute })
            .OrderBy(entry => entry.Attribute.TemplateKey)
            .ToList();
    }

    public static (RazorTemplateRenderer Renderer, IEmailTransport Transport) BuildServices()
    {
        var emailSettings = new EmailSettings
        {
            SmtpHost = "smtp.edu.gov.qa",
            SmtpPort = 25,
            EmailUser = "careers@edu.gov.qa",
            EmailPass = "Taw@Theef",
            ManagerEmails = "manager@careers.local",
            ContactUsEmail = "contact@careers.local",
            ProductName = "Careers"
        };

        var appConfig = new AppConfigSettings
        {
            FrontendUrl = "http://localhost:5029",
            BackendUrl = "https://localhost:7212",
            BlobSignKey = "demo-tawtheef-blob-sign-key-1234567890abcdef",
            AdminEmail = "admin@careers.local",
            AdminEmails = ["admin@careers.local"],
            DefaultSignedUrlMinutes = 3
        };

        var graphSettings = new GraphEmailSettings
        {
            TenantId = "2dcae639-d4a4-4454-82c7-592ab66fc7bd",
            ClientId = "3e3a6832-f94c-4b9b-8821-69672cbbfb75",
            ClientSecret = "qN~8Q~8~jyHA6DkiVJLf.9qViJh5SoxCncT3Scjg",
            FromAddress = "careers@edu.gov.qa",
            SaveToSentItems = true,
            LogoPath = "Templates\\Assets\\logo-en.jpg",
            LogoUrl = "assets/img/logo-black.png"
        };

        var branding = new DefaultBranding(Options.Create(emailSettings), Options.Create(appConfig));
        var renderer = new RazorTemplateRenderer(branding);

        // Use GraphEmailTransport to simulate server behavior (including the HTML fix)
        var logger = new SilentLogger();
        var httpClient = new HttpClient { BaseAddress = new Uri("https://graph.microsoft.com/v1.0/") };
        var graphMailer = new GraphMailer(httpClient, Options.Create(graphSettings), logger);

        var transport = new GraphEmailTransport(
            graphMailer,
            Options.Create(graphSettings),
            Options.Create(appConfig),
            new SimpleHttpClientFactory(httpClient),
            logger);

        return (renderer, transport);
    }
}

internal sealed class SimpleHttpClientFactory(HttpClient client) : IHttpClientFactory
{
    public HttpClient CreateClient(string name) => client;
}

internal sealed class SilentLogger : IAppLogger
{
    public IAppLogger ForContext(Type contextName) => this;
    public IAppLogger ForContext(string contextName) => this;
    public void Write(AppLogLevel level, string messageTemplate, params object?[] propertyValues) { }
    public void Write(Exception exception, AppLogLevel level, string messageTemplate, params object?[] propertyValues) { }
    public void Verbose(string messageTemplate, params object?[] propertyValues) { }
    public void Verbose(Exception exception, string messageTemplate, params object?[] propertyValues) { }
    public void Debug(string messageTemplate, params object?[] propertyValues) { }
    public void Debug(Exception exception, string messageTemplate, params object?[] propertyValues) { }
    public void Information(string messageTemplate, params object?[] propertyValues) { }
    public void Information(Exception exception, string messageTemplate, params object?[] propertyValues) { }
    public void Warning(string messageTemplate, params object?[] propertyValues) { }
    public void Warning(Exception exception, string messageTemplate, params object?[] propertyValues) { }
    public void Error(string messageTemplate, params object?[] propertyValues) { }
    public void Error(Exception exception, string messageTemplate, params object?[] propertyValues) { }
    public void Error(IEnumerable<FluentResults.IError> errors, string messageTemplate, params object?[] propertyValues) { }
    public void Fatal(string messageTemplate, params object?[] propertyValues) { }
    public void Fatal(Exception exception, string messageTemplate, params object?[] propertyValues) { }
}
