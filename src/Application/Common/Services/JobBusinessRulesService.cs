using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment;

namespace Tawtheef.Application.Common.Services;

public static class JobBusinessRules
{
    public static bool AreRequiredBasicFieldsCompleted(
        string titleAr, string titleEn,
        Guid sectorId, Guid managementId, Guid departmentId,
        Guid jobCategoryId, Guid workLocationId, Guid workTypeId,
        Guid majorId, int numberOfVacancies, DateTimeOffset closingDate,
        int minimumAge, int maximumAge, int yearsOfExperience)
    {
        return !string.IsNullOrWhiteSpace(titleAr) &&
               !string.IsNullOrWhiteSpace(titleEn) &&
               sectorId != Guid.Empty &&
               managementId != Guid.Empty &&
               departmentId != Guid.Empty &&
               jobCategoryId != Guid.Empty &&
               workLocationId != Guid.Empty &&
               workTypeId != Guid.Empty &&
               majorId != Guid.Empty &&
               numberOfVacancies > 0 &&
               closingDate > DateTimeOffset.Now &&
               minimumAge > 0 &&
               maximumAge > 0 &&
               maximumAge > minimumAge &&
               yearsOfExperience >= 0;
    }

    public static bool IsValidVacancyCount(int numberOfVacancies)
    {
        return numberOfVacancies > 0;
    }

    public static bool IsValidClosingDate(DateTimeOffset closingDate)
    {
        return closingDate.Date > DateTimeOffset.Now;
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

    public static bool HasDuplicatePointsSetup(Guid departmentId, Guid? subMajorId, ICollection<Job> existingJobs)
    {
        return existingJobs.Any(j =>
            j.DepartmentId == departmentId &&
            j.SubMajorId == subMajorId &&
            j.JobStatusId == JobStatusIds.Approved &&
            !j.IsDeleted);
    }

    public static bool IsDuplicateJob(
        string titleAr, Guid departmentId, Guid jobCategoryId, Guid? subMajorId,
        string existingTitleAr, Guid existingDepartmentId,
        Guid existingJobCategoryId, Guid? existingSubMajorId)
    {
        return titleAr.Equals(existingTitleAr, StringComparison.OrdinalIgnoreCase) &&
               departmentId == existingDepartmentId &&
               jobCategoryId == existingJobCategoryId &&
               subMajorId == existingSubMajorId;
    }

    public static bool CanEdit(Guid jobStatusId)
    {
        return jobStatusId == JobStatusIds.Draft || jobStatusId == JobStatusIds.NeedUpdate;
    }

    public static bool AreAllTabsCompleted(
        bool hasDegrees, bool hasConditions,
        bool hasResponsibilities, bool hasOverview, bool hasBenefits)
    {
        return hasDegrees && hasConditions && hasResponsibilities && hasOverview && hasBenefits;
    }

    public static bool IsInApprovalProcess(Guid jobStatusId)
    {
        return jobStatusId == JobStatusIds.PendingApproval;
    }

    public static bool CanPublish(Guid jobStatusId)
    {
        return jobStatusId == JobStatusIds.Approved;
    }

    public static bool IsValidTabItem(string text)
    {
        return !string.IsNullOrWhiteSpace(text);
    }

    public static bool IsDuplicateItem<T>(IEnumerable<T> items, T newItem, Func<T, string?> selector)
    {
        var newItemText = selector(newItem)?.Trim();
        if (string.IsNullOrEmpty(newItemText))
            return false;

        return items.Any(item => selector(item)?.Trim().Equals(newItemText, StringComparison.OrdinalIgnoreCase) == true);
    }

    public static bool CanModifyTabItems(Guid jobStatusId)
    {
        return jobStatusId == JobStatusIds.Draft;
    }

    public static bool CanShowSkillToApplicants(bool showToApplicants, Guid jobStatusId)
    {
        return jobStatusId == JobStatusIds.Draft || showToApplicants;
    }

    public static bool CanChangeSkillVisibility(Guid jobStatusId)
    {
        return jobStatusId == JobStatusIds.Draft;
    }

    public static bool IsValidAttachmentTitle(string title)
    {
        return !string.IsNullOrWhiteSpace(title);
    }

    public static bool IsDuplicateAttachment(IEnumerable<string> existingTitles, string newTitle)
    {
        return existingTitles.Contains(newTitle.Trim(), StringComparer.OrdinalIgnoreCase);
    }

    public static bool CanConfigurePoints(Guid jobStatusId)
    {
        return jobStatusId == JobStatusIds.Approved;
    }

    public static bool CanSendInvitations(Job job)
    {
        return job.JobStatusId == JobStatusIds.ReadyForAnnouncement;
    }

    public static bool CanPublishWithPoints(Job job)
    {
        return job.JobStatusId == JobStatusIds.Approved;
    }

    public static bool ShouldAutoClose(DateTime closingDate)
    {
        return closingDate < DateTime.Now;
    }

    public static bool CanApply(Job job)
    {
        return job.ClosingDate >= DateTime.Now && job.JobStatusId == JobStatusIds.Published;
    }

    public static bool CanCancel(Job job)
    {
        var finalStates = new[] { JobStatusIds.Closed, JobStatusIds.Cancelled };
        return job.ClosingDate > DateTime.Now && !finalStates.Contains(job.JobStatusId);
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

    public static bool CanSubmitForApproval(Guid jobStatusId, bool allTabsCompleted)
    {
        return jobStatusId == JobStatusIds.Draft && allTabsCompleted;
    }

    public static bool HasDuplicatePendingApproval(string titleAr, Guid departmentId, Guid jobCategoryId, Guid? subMajorId, ICollection<Job> existingJobs)
    {
        return existingJobs.Any(j =>
            j.TitleAr.Equals(titleAr, StringComparison.OrdinalIgnoreCase) &&
            j.DepartmentId == departmentId &&
            j.JobCategoryId == jobCategoryId &&
            j.SubMajorId == subMajorId &&
            j.JobStatusId == JobStatusIds.PendingApproval &&
            !j.IsDeleted);
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
