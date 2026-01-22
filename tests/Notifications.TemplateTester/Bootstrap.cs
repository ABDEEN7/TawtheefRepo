using System.Reflection;
using Microsoft.Extensions.Options;
using Tawtheef.Application.Common.Interfaces.NotificationServices;
using Tawtheef.Domain.Configurations.Settings;
using Tawtheef.Notifications.Attributes;
using Tawtheef.Notifications.Context;
using Tawtheef.Notifications.Services;

namespace Tawtheef.Notifications.TemplateTester;

internal static class Bootstrap
{
    public static List<TemplateEntry> LoadTemplates(Assembly asm)
    {
        return asm.GetTypes()
            .Where(type => type is { IsAbstract: false, IsInterface: false })
            .Select(type => new TemplateEntry(type, type.GetCustomAttribute<NotificationTemplateAttribute>()!))
            .Where(entry => entry.Attribute is not null)
            .Select(entry => entry with { Attribute = entry.Attribute! })
            .OrderBy(entry => entry.Attribute.TemplateKey)
            .ToList();
    }

    public static (RazorTemplateRenderer Renderer, IEmailTransport Transport) BuildServices()
    {
        var emailSettings = new EmailSettings
        {
            SmtpHost = "smtp.edu.gov.qa",
            SmtpPort = 25,
            EmailUser = "tawtheef@edu.gov.qa",
            EmailPass = "Taw@Theef", // Keep as-is or load from secrets
            ManagerEmails = "manager@tawtheef.local",
            ContactUsEmail = "contact@tawtheef.local",
            ProductName = "Tawtheef"
        };

        var appConfig = new AppConfigSettings
        {
            FrontendUrl = "https://localhost",
            BackendUrl = "https://localhost",
            BlobSignKey = "dev",
            AdminEmail = "admin@tawtheef.local",
            AdminEmails = ["admin@tawtheef.local"],
            DefaultSignedUrlMinutes = 3
        };

        var branding = new DefaultBranding(Options.Create(emailSettings), Options.Create(appConfig));
        var renderer = new RazorTemplateRenderer(branding);
        var transport = new FileEmailTransport(emailSettings);

        return (renderer, transport);
    }
}
