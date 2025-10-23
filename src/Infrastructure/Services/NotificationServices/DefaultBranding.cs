using Microsoft.Extensions.Options;
using Tawtheef.Application.Common.Interfaces.NotificationServices;
using Tawtheef.Domain.Configurations;

namespace Tawtheef.Infrastructure.Services.NotificationServices;

public sealed class DefaultBranding(IOptions<EmailSettings> emailConfiguration, IOptions<AppConfigSettings> appConfiguration) : IEmailBranding
{
    private readonly EmailSettings _emailConfig = emailConfiguration.Value;
    private readonly AppConfigSettings _appConfig = appConfiguration.Value;
    public string ProductName => _emailConfig.ProductName;
    public string PrimaryHex => "#667eea";
    public string SupportEmail => _emailConfig.DefaultReplyTo ?? _emailConfig.EmailUser;
    public string LogoUrl => "logo@tawtheef";
    public string WebsiteUrl => _appConfig.FrontendUrl;
}
