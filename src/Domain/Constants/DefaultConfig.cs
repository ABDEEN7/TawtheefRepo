namespace Tawtheef.Domain.Constants;

public static class DefaultConfig
{
    public const string DefaultLanguage = "ar";
    public const string DefaultCurrency = "USD";
    public const string DefaultTimeZone = "UTC";
    public const string DefaultDateFormat = "yyyy-MM-dd";
    public const string DefaultDateTimeFormat = "yyyy-MM-dd HH:mm:ss";
    public const int DefaultPageSize = 10;
    public const int MaxPageSize = 100;
    public const string DefaultImagePath = "/images/default.png";
    public static DateTime DefaultCreatedDate = new DateTime(1900, 1, 1);
}
