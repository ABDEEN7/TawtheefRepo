using Application.Operation.Common.Validations;
using Application.Operation.Features.Employee.JobManagement.Job.DTOs;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Services;
using Tawtheef.Application.Extensions;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;
using JobEntity = Tawtheef.Domain.Entities.Recruitment.Job;

namespace Tawtheef.Infrastructure.Services.Validations;

public class JobValidationService(IUnitOfWork unitOfWork) : IJobValidationService
{

    public async Task<ValidationResult> ValidateForCreation(CreateJobDto dto)
    {
        var failures = new List<ValidationFailure>();
        var degreeIds = dto.Degrees?.Select(d => d.DegreeId).ToList();
        AddBasicFieldFailures(failures,
            dto.JobTitleId,
            dto.SectorId, dto.ManagementId,
            dto.JobCategoryId, dto.WorkLocationId, dto.WorkTypeId,
            dto.MajorId, dto.NumberOfVacancies, dto.ClosingDate,
            dto.MinimumAge, dto.MaximumAge, dto.YearsOfExperience,
            dto.GenderId,
            degreeIds,
            isCreation: true);

        var ageValidation = JobBusinessRules.ValidateAgeRange(
            dto.MinimumAge, dto.MaximumAge, 18, 65);

        if (!ageValidation.IsValid)
            failures.Add(new ValidationFailure("AgeRange", ageValidation.ErrorMessage));

        if (dto.GenderId.HasValue && 
            await IsDuplicateJob(null, dto.ManagementId, dto.SectorId, dto.JobTitleId, dto.GenderId!.Value, dto.DepartmentId))
            failures.Add(new ValidationFailure("Duplicate", JobMessages.DuplicateJob));

        var hierarchicalErrors = await ValidateHierarchicalRelationships(dto);
        failures.AddRange(hierarchicalErrors);

        return failures.Count != 0 ? new ValidationResult(failures) : new ValidationResult();
    }
    private async Task<List<ValidationFailure>> ValidateSkillsByMajor(UpdateJobDto dto)
    {
        var failures = new List<ValidationFailure>();

        if (dto.Skills == null || !dto.Skills.Any() || dto.MajorId == null)
            return failures;

        var majorIds = new[] { dto.MajorId, dto.SubMajorId }
            .Where(id => id != null)
            .ToList();

        // Get skills linked to major/submajor
        var majorSkillIds = await unitOfWork
            .GetEntityRepository<MajorSkill>()
            .DbSet
            .AsNoTracking()
            .Where(ms => ms.IsActive && majorIds.Contains(ms.MajorId))
            .Select(ms => ms.SkillId)
            .ToListAsync();

        // Get general skills (no major restriction)
        var generalSkillIds = await unitOfWork
            .GetEntityRepository<Skill>()
            .DbSet
            .AsNoTracking()
            .Where(s => s.IsGeneral)
            .Select(s => s.Id)
            .ToListAsync();

        // Merge both valid sets
        var validSkillIds = majorSkillIds
            .Concat(generalSkillIds)
            .ToHashSet();

        var hasInvalidSkills = dto.Skills
            .Any(s => !validSkillIds.Contains(s.SkillId));

        if (hasInvalidSkills)
        {
            failures.Add(new ValidationFailure(
                nameof(dto.Skills),
                JobMessages.SkillNotInMajor));
        }

        return failures;
    }

    public async Task<bool> IsDuplicateJob(Guid? jobId, Guid managementId, Guid sectorId, Guid jobTitleId,
        Guid genderId, Guid? departmentId = null)
    {
        return await unitOfWork.GetEntityRepository<JobEntity>().DbSet
            .WhereIf(jobId is not null, j => j.Id != jobId)
            .AnyAsync(j =>
                j.JobStatusId != JobStatusIds.Cancelled &&
                j.JobStatusId != JobStatusIds.Closed &&
                j.JobStatusId != JobStatusIds.Rejected &&

                j.ManagementId == managementId &&
                j.SectorId == sectorId &&
                j.DepartmentId == departmentId &&

                j.JobTitleId == jobTitleId &&

                j.GenderId == genderId);
    }

    public async Task<ValidationResult> ValidateForUpdate(UpdateJobDto dto, JobEntity existingJob)
    {
        var failures = new List<ValidationFailure>();

        if ((dto.MinimumAge != existingJob.MinimumAge ||
             dto.MaximumAge != existingJob.MaximumAge) &&
            !JobBusinessRules.CanModifyAgeRange(existingJob.JobStatusId))
        {
            failures.Add(new ValidationFailure("AgeRange", JobMessages.CannotModifyAgeRange));
        }

        if (!JobBusinessRules.CanEdit(existingJob.JobStatusId))
        {
            failures.Add(new ValidationFailure("EditMode", JobMessages.CanOnlyEditInDraft));
        }

        if (JobBusinessRules.IsInApprovalProcess(existingJob.JobStatusId))
        {
            failures.Add(new ValidationFailure("ApprovalStatus", JobMessages.CannotEditInApproval));
        }

        if (dto.Conditions?.Any(c => string.IsNullOrWhiteSpace(c.TextAr)) == true)
            failures.Add(new ValidationFailure("Condition.TextAr", JobMessages.ConditionTextRequired));

        if (dto.Conditions?.Any(c => string.IsNullOrWhiteSpace(c.TextEn)) == true)
            failures.Add(new ValidationFailure("Condition.TextEn", JobMessages.ConditionTextRequired));

        if (dto.Responsibilities?.Any(r => string.IsNullOrWhiteSpace(r.TextAr)) == true)
            failures.Add(new ValidationFailure("Responsibility.TextAr", JobMessages.ResponsibilityTextRequired));

        if (dto.Responsibilities?.Any(r => string.IsNullOrWhiteSpace(r.TextEn)) == true)
            failures.Add(new ValidationFailure("Responsibility.TextEn", JobMessages.ResponsibilityTextRequired));

        if (dto.Conditions != null && HasDuplicates(dto.Conditions, c => c.TextAr))
            failures.Add(new ValidationFailure("Conditions", JobMessages.DuplicateCondition));

        if (dto.Conditions != null && HasDuplicates(dto.Conditions, c => c.TextEn))
            failures.Add(new ValidationFailure("Conditions", JobMessages.DuplicateCondition));

        if (dto.Responsibilities != null && HasDuplicates(dto.Responsibilities, r => r.TextAr))
            failures.Add(new ValidationFailure("Responsibilities", JobMessages.DuplicateResponsibility));

        if (dto.Responsibilities != null && HasDuplicates(dto.Responsibilities, r => r.TextEn))
            failures.Add(new ValidationFailure("Responsibilities", JobMessages.DuplicateResponsibility));

        if (dto.RequiredAttachments != null && HasDuplicates(dto.RequiredAttachments, a => a.TitleAr))
            failures.Add(new ValidationFailure("RequiredAttachments", JobMessages.DuplicateAttachment));

        if (dto.RequiredAttachments != null && HasDuplicates(dto.RequiredAttachments, a => a.TitleEn))
            failures.Add(new ValidationFailure("RequiredAttachments", JobMessages.DuplicateAttachment));

        if (dto.RequiredAttachments?.Any(a => string.IsNullOrWhiteSpace(a.TitleAr)) == true)
            failures.Add(new ValidationFailure("Attachment.TitleAr", JobMessages.AttachmentTitleRequired));

        if (dto.RequiredAttachments?.Any(a => string.IsNullOrWhiteSpace(a.TitleEn)) == true)
            failures.Add(new ValidationFailure("Attachment.TitleEn", JobMessages.AttachmentTitleRequired));

        if ((dto.JobTitleId != existingJob.JobTitleId) &&
            !JobBusinessRules.CanModifyTitle(existingJob.JobStatusId))
        {
            failures.Add(new ValidationFailure("Title", JobMessages.CannotModifyTitle));
        }

        if (HaveDegreesChanged(dto.Degrees, existingJob.JobDegrees) &&
            !JobBusinessRules.CanModifyQualifications(existingJob.JobStatusId))
        {
            failures.Add(new ValidationFailure("Degrees", JobMessages.CannotModifyQualifications));
        }

        if (HaveSkillsChanged(dto.Skills, existingJob.JobSkills) &&
            !JobBusinessRules.CanModifySkills(existingJob.JobStatusId))
        {
            failures.Add(new ValidationFailure("Skills", JobMessages.CannotModifySkills));
        }

        var skillMajorErrors = await ValidateSkillsByMajor(dto);
        failures.AddRange(skillMajorErrors);

        // Referential integrity: verify that all provided IDs exist in the DB
        var referentialErrors = await ValidateReferentialIntegrity(dto);
        failures.AddRange(referentialErrors);

        return failures.Count != 0 ? new ValidationResult(failures) : new ValidationResult();
    }

    public async Task<ValidationResult> ValidateStatusChange(JobEntity job, Guid newStatusId)
    {
        var failures = new List<ValidationFailure>();
        var degreeIds = job.JobDegrees.Select(d => d.DegreeId).ToList();
        if (newStatusId == JobStatusIds.PendingApproval &&
            (job.JobStatusId == JobStatusIds.Draft || job.JobStatusId == JobStatusIds.NeedUpdate))
        {
            AddBasicFieldFailures(failures,
                job.JobTitleId,
                job.SectorId, job.ManagementId,
                job.JobCategoryId, job.WorkLocationId, job.WorkTypeId,
                job.MajorId, job.NumberOfVacancies, job.ClosingDate,
                job.MinimumAge, job.MaximumAge, job.YearsOfExperience,
                job.GenderId,
                degreeIds);

            var allTabsCompleted = JobBusinessRules.AreAllTabsCompleted(
                job.JobDegrees is {Count: > 0},
                job.JobConditions is {Count: > 0},
                job.JobResponsibilities is {Count: > 0},
                HasQualificationDescriptions(job),
                HasOverview(job)
            );

            if (!allTabsCompleted)
                failures.Add(new ValidationFailure("TabsCompletion", JobMessages.AllTabsRequired));
        }

        if (newStatusId == JobStatusIds.PendingPointConfiguration)
        {
            AddBasicFieldFailures(failures,
                job.JobTitleId,
                job.SectorId, job.ManagementId,
                job.JobCategoryId, job.WorkLocationId, job.WorkTypeId,
                job.MajorId, job.NumberOfVacancies, job.ClosingDate,
                job.MinimumAge, job.MaximumAge, job.YearsOfExperience,
                job.GenderId,
                degreeIds);

            var allTabsCompleted = JobBusinessRules.AreAllTabsCompleted(
                job.JobDegrees is {Count: > 0},
                job.JobConditions is {Count: > 0},
                job.JobResponsibilities is {Count: > 0},
                HasQualificationDescriptions(job),
                HasOverview(job)
            );

            if (!allTabsCompleted)
                failures.Add(new ValidationFailure("TabsCompletion", JobMessages.AllTabsRequired));

            if (await IsDuplicateJob(job.Id, job.ManagementId, job.SectorId, job.JobTitleId, job.GenderId!.Value, job.DepartmentId))
                failures.Add(new ValidationFailure("Duplicate", JobMessages.DuplicateJob));
        }

        if (newStatusId == JobStatusIds.Published &&
            job.JobStatusId != JobStatusIds.ReadyForAnnouncement)
        {
            failures.Add(new ValidationFailure(
                "Status",
                JobMessages.CanOnlyPublishApproved));
        }

        if (newStatusId == JobStatusIds.Closed)
        {
            if (job.ClosingDate > DateTimeOffset.UtcNow)
            {
                failures.Add(new ValidationFailure(
                    nameof(job.ClosingDate),
                    JobMessages.CannotCloseManuallyBeforeDeadline));
            }
        }

        if (newStatusId == JobStatusIds.Cancelled)
        {
            var finalStates = new[] { JobStatusIds.Closed, JobStatusIds.Cancelled };
            if (finalStates.Contains(job.JobStatusId))
            {
                failures.Add(new ValidationFailure(
                    "Status",
                    JobMessages.CannotCancelFinalState));
            }
        }

        if (!JobBusinessRules.IsValidStatusTransition(job.JobStatusId, newStatusId))
        {
            failures.Add(new ValidationFailure(
                "StateMachine",
                JobMessages.InvalidStatusTransition));
        }

        return await Task.FromResult(failures.Count != 0
            ? new ValidationResult(failures)
            : new ValidationResult());
    }

    private static bool HasOverview(JobEntity job)
    {
        return !string.IsNullOrWhiteSpace(job.OverViewAr) &&
               !string.IsNullOrWhiteSpace(job.OverViewEn);
    }

    private static bool HasQualificationDescriptions(JobEntity job)
    {
        return !string.IsNullOrWhiteSpace(job.QualificationDescriptionAr) &&
               !string.IsNullOrWhiteSpace(job.QualificationDescriptionEn);
    }

    private async Task<List<ValidationFailure>> ValidateReferentialIntegrity(UpdateJobDto dto)
    {
        var failures = new List<ValidationFailure>();

        // Validate SkillIds exist
        if (dto.Skills is { Count: > 0 })
        {
            var requestedSkillIds = dto.Skills.Select(s => s.SkillId).Distinct().ToList();
            var existingSkillIds = await unitOfWork.GetEntityRepository<Skill>().DbSet
                .AsNoTracking()
                .Where(s => requestedSkillIds.Contains(s.Id))
                .Select(s => s.Id)
                .ToListAsync();

            if (existingSkillIds.Count != requestedSkillIds.Count)
                failures.Add(new ValidationFailure(nameof(dto.Skills), JobMessages.InvalidSkillReference));
        }

        // Validate DegreeIds exist
        if (dto.Degrees is { Count: > 0 })
        {
            var requestedDegreeIds = dto.Degrees.Select(d => d.DegreeId).Distinct().ToList();
            var existingDegreeIds = await unitOfWork.GetEntityRepository<Degree>().DbSet
                .AsNoTracking()
                .Where(d => requestedDegreeIds.Contains(d.Id))
                .Select(d => d.Id)
                .ToListAsync();

            if (existingDegreeIds.Count != requestedDegreeIds.Count)
                failures.Add(new ValidationFailure(nameof(dto.Degrees), JobMessages.InvalidDegreeReference));
        }

        // Validate Specialization MajorIds / SubMajorIds exist
        if (dto.JobSpecializations is { Count: > 0 })
        {
            var allMajorIds = dto.JobSpecializations
                .Select(s => s.MajorId)
                .Concat(dto.JobSpecializations
                    .Where(s => s.SubMajorId.HasValue)
                    .Select(s => s.SubMajorId!.Value))
                .Distinct()
                .ToList();

            var existingMajorIds = await unitOfWork.GetEntityRepository<Major>().DbSet
                .AsNoTracking()
                .Where(m => allMajorIds.Contains(m.Id))
                .Select(m => m.Id)
                .ToListAsync();

            if (existingMajorIds.Count != allMajorIds.Count)
                failures.Add(new ValidationFailure(nameof(dto.JobSpecializations), JobMessages.InvalidSpecializationMajorReference));
        }

        return failures;
    }

    
    private bool HasDuplicates<T>(IEnumerable<T> items, Func<T, string> selector)
    {
        var texts = items.Select(selector).Where(t => !string.IsNullOrWhiteSpace(t));
        IEnumerable<string> enumerable = texts as string[] ?? texts.ToArray();
        return enumerable.Distinct(StringComparer.OrdinalIgnoreCase).Count() != enumerable.Count();
    }

    private bool HaveDegreesChanged(List<JobDegreeRequestDto>? newDegrees, ICollection<JobDegree>? existingDegrees)
    {
        if (newDegrees == null && existingDegrees?.Any() != true) return false;
        if (newDegrees?.Count != existingDegrees?.Count) return true;

        var newIds = newDegrees?.Select(d => d.DegreeId).OrderBy(id => id).ToList();
        var existingIds = existingDegrees?.Select(d => d.DegreeId).OrderBy(id => id).ToList();

        return !newIds?.SequenceEqual(existingIds ?? []) ?? false;
    }

    private bool HaveSkillsChanged(List<JobSkillRequestDto>? newSkills, ICollection<JobSkill>? existingSkills)
    {
        if (newSkills == null && existingSkills?.Any() != true) return false;
        if (newSkills?.Count != existingSkills?.Count) return true;

        var newIds = newSkills?.Select(s => s.SkillId).OrderBy(id => id).ToList();
        var existingIds = existingSkills?.Select(s => s.SkillId).OrderBy(id => id).ToList();

        return !newIds?.SequenceEqual(existingIds ?? []) ?? false;
    }

    private async Task<List<ValidationFailure>> ValidateHierarchicalRelationships(CreateJobDto dto)
    {
        var failures = new List<ValidationFailure>();

        if (dto.SectorId != Guid.Empty && dto.ManagementId != Guid.Empty)
        {
            var isValid = await IsManagementUnderSector(dto.ManagementId, dto.SectorId);
            if (!isValid)
                failures.Add(new ValidationFailure(
                    nameof(dto.ManagementId),
                    JobMessages.ManagementNotUnderSector));
        }

        if (dto.ManagementId != Guid.Empty && dto.DepartmentId is not null)
        {
            var isValid = await IsDepartmentUnderManagement(dto.DepartmentId!.Value, dto.ManagementId);
            if (!isValid)
                failures.Add(new ValidationFailure(
                    nameof(dto.DepartmentId),
                    JobMessages.DepartmentNotUnderManagement));
        }

        if (dto.MajorId is not null && dto.MajorId != Guid.Empty && dto.SubMajorId.HasValue)
        {
            var isValid = await IsSubMajorUnderMajor(dto.SubMajorId.Value, dto.MajorId!.Value);
            if (!isValid)
                failures.Add(new ValidationFailure(
                    nameof(dto.SubMajorId),
                    JobMessages.SubmajorNotUnderMajor));
        }

        return failures;
    }

    private async Task<bool> IsManagementUnderSector(Guid managementId, Guid sectorId)
    {
        var managementRepo = unitOfWork.GetEntityRepository<Management>();
        var management = await managementRepo.DbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == managementId && m.SectorId == sectorId);

        return management != null;
    }

    private async Task<bool> IsDepartmentUnderManagement(Guid departmentId, Guid managementId)
    {
        var departmentRepo = unitOfWork.GetEntityRepository<Department>();
        var department = await departmentRepo.DbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == departmentId && d.ManagementId == managementId);

        return department != null;
    }

    private async Task<bool> IsSubMajorUnderMajor(Guid subMajorId, Guid majorId)
    {
        var majorRepo = unitOfWork.GetEntityRepository<Major>();
        var subMajor = await majorRepo.DbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == subMajorId && m.ParentId == majorId);

        return subMajor != null;
    }

    private void AddBasicFieldFailures(
        List<ValidationFailure> failures,
        Guid jobTitleId,
        Guid sectorId, Guid managementId,
        Guid jobCategoryId, Guid workLocationId, Guid workTypeId,
        Guid? majorId, int numberOfVacancies, DateTimeOffset closingDate,
        int minimumAge, int maximumAge, int yearsOfExperience,
        Guid? genderId = null,
        List<Guid>? degreeIds = null,
        bool isCreation = false)
    {
        if (jobTitleId == Guid.Empty)
            failures.Add(new ValidationFailure(nameof(jobTitleId), JobMessages.JobTitleRequired));

        if (sectorId == Guid.Empty)
            failures.Add(new ValidationFailure(nameof(sectorId), JobMessages.SectorRequired));

        if (managementId == Guid.Empty)
            failures.Add(new ValidationFailure(nameof(managementId), JobMessages.ManagementRequired));

        if (jobCategoryId == Guid.Empty)
            failures.Add(new ValidationFailure(nameof(jobCategoryId), JobMessages.JobCategoryRequired));

        if (workLocationId == Guid.Empty)
            failures.Add(new ValidationFailure(nameof(workLocationId), JobMessages.WorkLocationRequired));

        if (genderId == null || genderId == Guid.Empty)
            failures.Add(new ValidationFailure(nameof(genderId), JobMessages.GenderRequired));

        if (workTypeId == Guid.Empty)
            failures.Add(new ValidationFailure(nameof(workTypeId), JobMessages.WorkTypeRequired));

        if (!isCreation && JobBusinessRules.RequiresMajor(degreeIds) && (majorId == null || majorId == Guid.Empty))
            failures.Add(new ValidationFailure(nameof(majorId), JobMessages.MajorRequired));

        if (numberOfVacancies <= 0)
            failures.Add(new ValidationFailure(nameof(numberOfVacancies), JobMessages.VacanciesGreaterThanZero));

        if (closingDate <= DateTimeOffset.Now)
            failures.Add(new ValidationFailure(nameof(closingDate), JobMessages.ClosingDateFuture));

        if (minimumAge <= 0)
            failures.Add(new ValidationFailure(nameof(minimumAge), JobMessages.MinimumAgeRequired));

        if (maximumAge <= 0)
            failures.Add(new ValidationFailure(nameof(maximumAge), JobMessages.MaximumAgeRequired));

        if (maximumAge <= minimumAge)
            failures.Add(new ValidationFailure(nameof(maximumAge), JobMessages.AgeRangeInvalid));

        if (yearsOfExperience < 0)
            failures.Add(new ValidationFailure(nameof(yearsOfExperience), JobMessages.YearsExperienceRequired));
    }
}
