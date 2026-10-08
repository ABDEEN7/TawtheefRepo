using Tawtheef.Domain.Entities.Notification;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Common.Notifications;

public static class ExamExecutionNotificationChannels
{
    public static string Key(string eventCode, NotificationChannel channel, Guid entityId, Guid recipientId)
    {
        var channelCode = channel switch
        {
            NotificationChannel.InApp => "ia",
            NotificationChannel.Email => "em",
            NotificationChannel.Sms => "sm",
            _ => throw new ArgumentOutOfRangeException(nameof(channel))
        };
        var key = $"{eventCode}-{channelCode}-{entityId:N}-{recipientId:N}";
        if (key.Length > 100)
            throw new ArgumentOutOfRangeException(nameof(eventCode), "Notification idempotency key exceeds 100 characters.");
        return key;
    }

    public static IEnumerable<Notification> Create(string templateKey, User recipient, string payloadJson,
        string eventCode, Guid entityId, bool candidateCritical)
    {
        var language = recipient.GetPreferredLanguage();
        yield return Notification.Create(NotificationChannel.InApp, templateKey, recipient.Id,
            recipient.Email, payloadJson: payloadJson, language: language,
            idempotencyKey: Key(eventCode, NotificationChannel.InApp, entityId, recipient.Id));

        if (!string.IsNullOrWhiteSpace(recipient.Email))
            yield return Notification.Create(NotificationChannel.Email, templateKey, recipient.Id,
                recipient.Email, payloadJson: payloadJson, language: language,
                idempotencyKey: Key(eventCode, NotificationChannel.Email, entityId, recipient.Id));

        if (candidateCritical && !string.IsNullOrWhiteSpace(recipient.PhoneNumber))
            yield return Notification.Create(NotificationChannel.Sms, templateKey, recipient.Id,
                recipient.PhoneNumber, payloadJson: payloadJson, language: language,
                idempotencyKey: Key(eventCode, NotificationChannel.Sms, entityId, recipient.Id));
    }
}
