namespace Application.Recruitment.Features.Authenticator.Handlers.Commands.QatarResidentOtp;

internal static class QatarResidentOtpConstants
{
    public const string Provider = "QatarResidentOtp";
    public const string DisplayName = "Qatar Resident";
    public const string PlaceholderEmailDomain = "@login.local";
    public const int OtpLength = 6;
    public const int OtpExpiryMinutes = 10;
    public const int MaxOtpAttempts = 5;
    public const int MaxOtpSends = 5;
    public static readonly TimeSpan OtpLockDuration = TimeSpan.FromMinutes(10);
    public static readonly TimeSpan OtpSendWindow = TimeSpan.FromMinutes(15);
}
