using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Notification;

public enum NotificationChannel { Sms = 1, Email = 2, Push = 3, InApp = 4 }
public enum NotificationStatus { Pending = 1, Queued = 2, Sent = 3, Failed = 4, Canceled = 5 }

[Index(nameof(Status), nameof(Channel))]
[Index(nameof(UserId), nameof(Channel), nameof(IsDismissed), nameof(IsRead))]
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

    // Matches EmailTemplate.TemplateKey
    [MaxLength(100)]
    public string TemplateKey { get; private set; } = string.Empty;

    // Subject / title (email, push title, etc.)
    [MaxLength(255)]
    public string? Subject { get; private set; }

    // Message body (email HTML, SMS text, push body)
    [MaxLength(int.MaxValue)]
    public string? Body { get; private set; }

    [MaxLength(int.MaxValue)]
    public string? PlainTextBody { get; private set; }

    // Serialized payload (JSON metadata, variables, deep links, etc.)
    [MaxLength(int.MaxValue)]
    public string? PayloadJson { get; private set; }

    // Provider-side ID (SendGrid, Twilio, Firebase, etc.)
    [MaxLength(100)]
    public string? ProviderMessageId { get; set; }

    // Unique key to prevent duplicates
    [MaxLength(100)]
    public string? IdempotencyKey { get; private set; }

    public int RetryCount { get; private set; } = 0;
    public int MaxRetries { get; private set; } = 3; // Default to 3 retries
    public DateTime? NextRetryAt { get; private set; }

    public NotificationStatus Status { get; set; } = NotificationStatus.Pending;

    // Error / failure reason
    [MaxLength(1000)]
    public string? Error { get; private set; }

    public DateTime? SentAt { get; private set; }

    public bool IsRead { get; private set; }
    public bool IsDismissed { get; private set; }

    [MaxLength(10)]
    public string Language { get; private set; } = "ar";

    public void MarkAsRead() => IsRead = true;
    public void MarkAsUnread() => IsRead = false;
    public void Dismiss() => IsDismissed = true;

    public static Notification Create(
        NotificationChannel channel, string templateKey, Guid? userId,
        string? toAddress, string? subject, string? body, string? plainTextBody,
        string? payloadJson, string language, string? idempotencyKey = null, int maxRetries = 3)
    {
        return new Notification
        {
            Channel = channel, 
            TemplateKey = templateKey,
            UserId = userId, 
            ToAddress = toAddress,
            Subject = subject, 
            Body = body, 
            PlainTextBody = plainTextBody,
            PayloadJson = payloadJson,
            Status = NotificationStatus.Pending,
            IdempotencyKey = idempotencyKey,
            MaxRetries = maxRetries,
            Language = language
        };
    }

    public void MarkSent(string? providerId, DateTime whenUtc)
    {
        ProviderMessageId = providerId;
        SentAt = whenUtc;
        Status = NotificationStatus.Sent;
        Error = null;
    }

    public void MarkFailed(string error, DateTime? nextRetry = null)
    {
        if (nextRetry.HasValue && RetryCount < MaxRetries)
        {
            Status = NotificationStatus.Pending;
            NextRetryAt = nextRetry;
            RetryCount++;
        }
        else
        {
            Status = NotificationStatus.Failed;
        }
        Error = error;
    }

    public void Cancel(string reason)
    {
        Status = NotificationStatus.Canceled;
        Error = reason;
    }
}

