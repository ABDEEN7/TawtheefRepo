namespace Tawtheef.Application.Extensions;

public static class DateTimeMappingExtensions
{
    public static DateTimeOffset AsUtcOffset(this DateTime utcDateTime)
        => new(DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc), TimeSpan.Zero);

    public static DateTimeOffset? AsUtcOffset(this DateTime? utcDateTime)
        => utcDateTime?.AsUtcOffset();
}
