using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Notification;

public enum NotificationChannel { Sms = 1, Email = 2, Push = 3, InApp = 4 }
public enum NotificationStatus { Pending = 1, Queued = 2, Sent = 3, Failed = 4, Canceled = 5 }

[Index(nameof(Status), nameof(Channel))]
[Index(nameof(UserId))]
public class Notification : EventEntity
{
    public Guid? UserId { get; private set; }
    /// <summary>
    /// Email address split by comma.
    /// </summary>
    [MaxLength(200)]
    public string? ToAddress { get; private set; }
    /// <summary>
    /// Email address split by comma.
    /// </summary>
    [MaxLength(200)]
    public string? CcAddress { get; private set; }
    public NotificationChannel Channel { get; private set; }
    [MaxLength(100)]
    public string TemplateKey { get; private set; } = string.Empty;
    [MaxLength(250)]
    public string? Subject { get; private set; }
    public string? Body { get; private set; }
    public string? PayloadJson { get; private set; }
    public string? ProviderMessageId { get; set; }
    public NotificationStatus Status { get; set; } = NotificationStatus.Pending;
    [MaxLength(1000)]
    public string? Error { get; private set; }
    public DateTime? SentAtUtc { get; private set; }

    public static Notification Create(
        NotificationChannel channel, string templateKey, Guid? userId,
        string? toAddress, string? subject, string? body, string? payloadJson)
    {
        return new Notification
        {
            Channel = channel, 
            TemplateKey = templateKey,
            UserId = userId, 
            ToAddress = toAddress,
            Subject = subject, 
            Body = body, 
            PayloadJson = payloadJson,
            Status = NotificationStatus.Pending
        };
    }

    public void MarkSent(string? providerId, DateTime whenUtc)
    {
        ProviderMessageId = providerId;
        SentAtUtc = whenUtc;
        Status = NotificationStatus.Sent;
        Error = null;
    }

    public void MarkFailed(string error)
    {
        Status = NotificationStatus.Failed;
        Error = error;
    }

    public void Cancel(string reason)
    {
        Status = NotificationStatus.Canceled;
        Error = reason;
    }
}
