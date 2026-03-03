using Tawtheef.Domain.Entities.Recruitment;

namespace Application.Operation.Features.Admin.ProfileLogs;

public static class ProfileLogSources
{
    public const string UserProfileLogger = nameof(UserProfileLogger);
    public const string AuditTrail = nameof(AuditTrailEntry);
    public const string ActionLog = nameof(ActionLog);
}
