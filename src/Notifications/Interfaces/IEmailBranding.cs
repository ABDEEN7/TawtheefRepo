namespace Tawtheef.Notifications.Interfaces;

public interface IEmailBranding
{
    string ProductNameEn { get; }
    string ProductNameAr { get; }
    string PrimaryHex { get; }
    string SupportEmail { get; }
    string LogoUrl { get; }
    string WebsiteUrl { get; }
    string RecruitmentUrl { get; }
    string OperationsUrl { get; }
}
