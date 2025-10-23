namespace Tawtheef.Application.Common.Interfaces.NotificationServices;

public interface IEmailBranding
{
    string ProductName { get; }
    string PrimaryHex { get; }
    string SupportEmail { get; }
    string LogoUrl { get; }
    string WebsiteUrl { get; }
}