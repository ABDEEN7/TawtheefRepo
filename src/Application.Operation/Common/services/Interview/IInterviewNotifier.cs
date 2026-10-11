using Tawtheef.Domain.Entities.Notification;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Common.Services.Interview;

/// <summary>
/// Shared plumbing for the interview workflow notification handlers: finding recipients and queueing
/// <see cref="Tawtheef.Domain.Entities.Notification.Notification"/> rows for the dispatcher.
/// </summary>
public interface IInterviewNotifier
{
    /// <summary>Active employee users holding <paramref name="permission"/> through a role, minus <paramref name="excludeUserId"/>.</summary>
    Task<IReadOnlyList<User>> GetEmployeesWithPermissionAsync(string permission, Guid? excludeUserId, CancellationToken ct);

    Task<User?> FindUserAsync(Guid? userId, CancellationToken ct);

    /// <summary>
    /// Adds one row per requested channel in the recipient's preferred language (Email / SMS are skipped when the
    /// recipient has no email / phone number). The model and subject are
    /// built for that language, so names and titles match the message. Does not save. Event handlers must call
    /// SaveChangesAsync themselves: events are dispatched from SavingChanges after AuditableEntityInterceptor has
    /// already run, so rows left for the command's own save would be stored without CreatedDate / CreatedById.
    /// </summary>
    Task QueueAsync(
        User recipient,
        string templateKey,
        Func<string, object> modelFor,
        Func<string, string>? subjectFor,
        IReadOnlyCollection<NotificationChannel> channels,
        CancellationToken ct);
}
