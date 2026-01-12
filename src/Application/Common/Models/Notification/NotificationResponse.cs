using FluentResults;

namespace Tawtheef.Application.Common.Models.Notification;

public sealed record NotificationResponse(bool Ok, string? ProviderId, IReadOnlyList<IError>? Errors)
{
    public static NotificationResponse Success(string providerId) =>
        new(true, providerId, null);

    public static NotificationResponse Failure(IReadOnlyList<IError> errors) =>
        new(false, null, errors);

    public static NotificationResponse Failure(IError error) =>
        new(false, null, new List<IError> { error });
    
    public static NotificationResponse Failure(string error) =>
        new(false, null, new List<IError> { new FluentResults.Error(error) });
}
