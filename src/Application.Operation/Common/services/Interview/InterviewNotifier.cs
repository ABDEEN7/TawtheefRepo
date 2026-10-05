using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Notification;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Common.Services.Interview;

public sealed class InterviewNotifier(
    UserManager<User> userManager,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork) : IInterviewNotifier
{
    private const int SubjectMaxLength = 255;

    public async Task<IReadOnlyList<User>> GetEmployeesWithPermissionAsync(
        string permission, Guid? excludeUserId, CancellationToken ct)
    {
        var permittedIds = await userRepository.GetUserIdsByPermissionAsync(permission, ct);
        if (permittedIds.Count == 0) return [];

        return await userManager.Users
            .AsNoTracking()
            .OfType<EmployeeUser>()
            .Where(u => !u.IsBlocked && !u.IsDeleted && permittedIds.Contains(u.Id))
            .Where(u => excludeUserId == null || u.Id != excludeUserId) // this for exclude the user who is performing the action (Ex: the one who created the template) : we can change it if BRD asked
            .Cast<User>()
            .ToListAsync(ct);
    }

    public async Task<User?> FindUserAsync(Guid? userId, CancellationToken ct)
    {
        if (userId is null) return null;
        return await userManager.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId, ct);
    }

    public async Task QueueAsync(
        User recipient,
        string templateKey,
        Func<string, object> modelFor,
        Func<string, string>? subjectFor,
        IReadOnlyCollection<NotificationChannel> channels,
        CancellationToken ct)
    {
        var lang = recipient.GetPreferredLanguage();
        var model = modelFor(lang);
        // Serialize with the runtime type: the dispatcher deserializes the payload into the template's model.
        var payload = JsonSerializer.Serialize(model, model.GetType());
        var subject = Truncate(subjectFor?.Invoke(lang));
        var repo = unitOfWork.GetEntityRepository<Notification>();

        foreach (var channel in channels.Distinct())
        {
            // Email / SMS need an address; InApp and Push are delivered by UserId (ToAddress kept as the email, like
            // the existing handlers).
            var toAddress = channel == NotificationChannel.Sms ? recipient.PhoneNumber : recipient.Email;
            if ((channel is NotificationChannel.Email or NotificationChannel.Sms) && string.IsNullOrWhiteSpace(toAddress))
                continue;

            await repo.AddAsync(Notification.Create(channel, templateKey, recipient.Id,
                toAddress, subject, null, null, payload, lang), ct);
        }
    }

    private static string? Truncate(string? value) =>
        value is { Length: > SubjectMaxLength } ? value[..(SubjectMaxLength - 1)] + "…" : value;
}
