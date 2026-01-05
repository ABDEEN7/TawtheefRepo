namespace Tawtheef.Application.Features.Notifications.DTOs;

public class UserNotificationDto
{
    public Guid Id { get; set; }
    public string? Subject { get; set; }
    public string? Body { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTimeOffset CreatedDate { get; set; }
    public DateTime? SentAtUtc { get; set; }
    public string? Error { get; set; }
}
