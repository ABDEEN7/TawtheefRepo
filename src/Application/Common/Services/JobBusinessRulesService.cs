using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment;

namespace Tawtheef.Application.Common.Services;

public static class JobBusinessRules
{
    private static readonly HashSet<Guid> SimplifiedDegrees =
    [
        DegreeIds.Secondary,
        DegreeIds.Preparatory,
        DegreeIds.Primary
    ];

    public static bool RequiresMajor(List<Guid>? degreeIds)
    {
        if (degreeIds == null || degreeIds.Count == 0) 
            return true;

        return degreeIds.Any(id => !SimplifiedDegrees.Contains(id));
    }

    public static (bool IsValid, string ErrorMessage) ValidateAgeRange(
        int minAge, int maxAge, int systemMinAge, int systemMaxAge)
    {
        if (minAge < systemMinAge)
            return (false, JobMessages.MinAgeSystemLimit);

        if (maxAge > systemMaxAge)
            return (false, JobMessages.MaxAgeSystemLimit);

        if (minAge >= maxAge)
            return (false, JobMessages.AgeRangeInvalid);

        return (true, string.Empty);
    }

    public static bool CanModifyAgeRange(Guid jobStatusId)
    {
        return jobStatusId == JobStatusIds.Draft;
    }

    public static bool CanEdit(Guid jobStatusId)
    {
        return jobStatusId == JobStatusIds.Draft || jobStatusId == JobStatusIds.NeedUpdate;
    }

    public static bool AreAllTabsCompleted(
        bool hasDegrees,
        bool hasConditions,
        bool hasResponsibilities,
        bool hasQualifications,
        bool hasOverview)
    {
        return hasDegrees
               && hasConditions
               && hasResponsibilities
               && hasQualifications
               && hasOverview;
    }

    public static bool IsInApprovalProcess(Guid jobStatusId)
    {
        return jobStatusId == JobStatusIds.PendingApproval;
    }

    public static bool CanModifyTitle(Guid jobStatusId)
    {
        return jobStatusId == JobStatusIds.Draft;
    }

    public static bool CanModifyQualifications(Guid jobStatusId)
    {
        return jobStatusId == JobStatusIds.Draft;
    }

    public static bool CanModifySkills(Guid jobStatusId)
    {
        return jobStatusId == JobStatusIds.Draft;
    }

    public static bool IsValidStatusTransition(Guid currentStatusId, Guid newStatusId)
    {
        var allowedTransitions = new Dictionary<Guid, List<Guid>>
        {
            [JobStatusIds.Draft] = [JobStatusIds.PendingApproval, JobStatusIds.Cancelled],
            [JobStatusIds.PendingApproval] = [JobStatusIds.NeedUpdate,JobStatusIds.Approved, JobStatusIds.Rejected, JobStatusIds.Cancelled],
            [JobStatusIds.Approved] = [JobStatusIds.ReadyForAnnouncement, JobStatusIds.Cancelled],
            [JobStatusIds.NeedUpdate]= [JobStatusIds.PendingApproval, JobStatusIds.Cancelled],
            [JobStatusIds.ReadyForAnnouncement] = [JobStatusIds.Published],
            [JobStatusIds.Published] = [JobStatusIds.Closed, JobStatusIds.Cancelled],
            [JobStatusIds.Rejected] = [JobStatusIds.Draft],
            [JobStatusIds.Closed] = [],
            [JobStatusIds.Cancelled] = []
        };

        return allowedTransitions.ContainsKey(currentStatusId) && allowedTransitions[currentStatusId].Contains(newStatusId);
    }
    
    public static bool CanCopyFromPreviousJob(Guid jobStatusId)
    {
        return jobStatusId == JobStatusIds.Approved ||
               jobStatusId == JobStatusIds.ReadyForAnnouncement ||
               jobStatusId == JobStatusIds.Published ||
               jobStatusId == JobStatusIds.Closed ||
               jobStatusId == JobStatusIds.Cancelled;
    }
}
