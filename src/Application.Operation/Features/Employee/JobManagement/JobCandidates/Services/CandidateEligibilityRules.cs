using Tawtheef.Domain.Entities.Lookups;

namespace Application.Operation.Features.Employee.JobManagement.JobCandidates.Services;

public static class CandidateEligibilityRules
{
    public static readonly Guid[] ActiveInvitationStatuses =
    {
        InvitationStatusIds.NewInvitation,
        InvitationStatusIds.Read,
        InvitationStatusIds.PendingAttachmentApproval,
        InvitationStatusIds.ReturnedAttachment,
        InvitationStatusIds.ExamEligible,
        InvitationStatusIds.Rejected
    };

    public static bool IsAgeWithinRange(DateOnly birthDate, int minAge, int maxAge)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var minBirthDate = today.AddYears(-maxAge);
        var maxBirthDate = today.AddYears(-minAge);
        return birthDate >= minBirthDate && birthDate <= maxBirthDate;
    }

    public static bool HasAllRequiredSkills(IEnumerable<Guid> candidateSkillIds, IEnumerable<Guid> requiredSkillIds)
    {
        var requiredList = requiredSkillIds.ToList();
        return requiredList.Count == 0 || requiredList.All(candidateSkillIds.Contains);
    }
}
