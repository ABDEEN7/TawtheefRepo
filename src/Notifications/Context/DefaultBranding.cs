using Microsoft.Extensions.Options;
using Tawtheef.Domain.Configurations.Settings;
using Tawtheef.Notifications.Interfaces;

namespace Tawtheef.Notifications.Context;

public sealed class DefaultBranding(
    IOptions<EmailSettings> emailConfiguration,
    IOptions<AppConfigSettings> appConfiguration) : IEmailBranding
{
    private readonly EmailSettings _emailConfig = emailConfiguration.Value;
    private readonly AppConfigSettings _appConfig = appConfiguration.Value;

    public string ProductName => _emailConfig.ProductName;

    // From the logo dominant color
    public string PrimaryHex => "#881038";

    public string SupportEmail => _emailConfig.DefaultReplyTo ?? _emailConfig.EmailUser;

    // Ensure this is a real URL reachable by email clients (https://...)
    public string LogoUrl => "logo@careers";

    public string WebsiteUrl => _emailConfig.RedirectUrl ?? _appConfig.FrontendUrl;
    public string RecruitmentUrl => _appConfig.ClientUrl ?? "";
    public string OperationsUrl => _appConfig.FrontendUrl;
}
