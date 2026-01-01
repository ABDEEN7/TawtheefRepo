using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Validations;
using Tawtheef.Application.Common.Services;
using Tawtheef.Application.Features.Operations.Employee.Job.DTOs;
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

        if (!JobBusinessRules.AreRequiredBasicFieldsCompleted(
            dto.TitleAr, dto.TitleEn,
            dto.SectorId, dto.ManagementId, dto.DepartmentId,
            dto.JobCategoryId, dto.WorkLocationId, dto.WorkTypeId,
            dto.MajorId, dto.NumberOfVacancies, dto.ClosingDate,
            dto.MinimumAge, dto.MaximumAge, dto.YearsOfExperience))
        {
            failures.Add(new ValidationFailure("BasicFields", JobMessages.FieldRequired));
        }

        if (!JobBusinessRules.IsValidVacancyCount(dto.NumberOfVacancies))
            failures.Add(new ValidationFailure(nameof(dto.NumberOfVacancies),
                JobMessages.VacanciesGreaterThanZero));

        if (!JobBusinessRules.IsValidClosingDate(dto.ClosingDate))
            failures.Add(new ValidationFailure(nameof(dto.ClosingDate),
                JobMessages.ClosingDateFuture));

        var ageValidation = JobBusinessRules.ValidateAgeRange(
            dto.MinimumAge, dto.MaximumAge, 18, 65);

        if (!ageValidation.IsValid)
            failures.Add(new ValidationFailure("AgeRange", ageValidation.ErrorMessage));

        if (await IsDuplicateJob(dto))
            failures.Add(new ValidationFailure("Duplicate", JobMessages.DuplicateJob));

        var hierarchicalErrors = await ValidateHierarchicalRelationships(dto);
        failures.AddRange(hierarchicalErrors);

        return failures.Count != 0 ? new ValidationResult(failures) : new ValidationResult();
    }

    private async Task<List<ValidationFailure>> ValidateSkillsByMajor(UpdateJobDto dto)
    {
        var failures = new List<ValidationFailure>();

        if (dto.Skills?.Any() == true && dto.MajorId != Guid.Empty)
        {
            var majorSkillRepo = unitOfWork.GetEntityRepository<MajorSkill>();
            var majorSkillsIds = await majorSkillRepo.DbSet
                .AsNoTracking().Where(s => s.IsActive)
                .Where(s => s.MajorId == dto.MajorId)
                .Select(x=> x.SkillId)
                .ToListAsync();


            var validSkillIds = majorSkillsIds.ToHashSet();

            var invalidSkillIds = dto.Skills
                .Where(s => !validSkillIds.Contains(s.SkillId))
                .Select(s => s.SkillId)
                .ToList();

            if (invalidSkillIds.Count != 0)
            {
                failures.Add(new ValidationFailure(
                    nameof(dto.Skills),
                    JobMessages.SkillNotInMajor));
            }
        }

        return failures;
    }
    private async Task<bool> IsDuplicateJob(CreateJobDto jobDto)
    {
         return await unitOfWork.GetEntityRepository<JobEntity>().DbSet
        .AnyAsync(j =>
            j.TitleAr == jobDto.TitleAr &&
            j.DepartmentId == jobDto.DepartmentId &&
            j.JobCategoryId == jobDto.JobCategoryId &&
            j.MajorId == jobDto.MajorId &&
            j.SubMajorId == jobDto.SubMajorId &&
            !j.IsDeleted);
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

        if ((dto.TitleAr != existingJob.TitleAr || dto.TitleEn != existingJob.TitleEn) &&
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

        return failures.Count != 0 ? new ValidationResult(failures) : new ValidationResult();
    }

    public Task<ValidationResult> ValidateStatusChange(JobEntity job, Guid newStatusId)
    {
        var failures = new List<ValidationFailure>();

        if (newStatusId == JobStatusIds.PendingApproval &&
            (job.JobStatusId == JobStatusIds.Draft || job.JobStatusId == JobStatusIds.NeedUpdate))
        {
            if (!JobBusinessRules.AreRequiredBasicFieldsCompleted(
                    job.TitleAr, job.TitleEn,
                    job.SectorId, job.ManagementId, job.DepartmentId,
                    job.JobCategoryId, job.WorkLocationId, job.WorkTypeId,
                    job.MajorId, job.NumberOfVacancies, job.ClosingDate,
                    job.MinimumAge, job.MaximumAge, job.YearsOfExperience))
            {
                failures.Add(new ValidationFailure("BasicFields", JobMessages.FieldRequired));
            }

            var allTabsCompleted = JobBusinessRules.AreAllTabsCompleted(
                job.JobDegrees.Any(),
                job.JobConditions.Any(),
                job.JobResponsibilities.Any(),
                !string.IsNullOrWhiteSpace(job.OverViewAr) && !string.IsNullOrWhiteSpace(job.OverViewEn),
                !string.IsNullOrWhiteSpace(job.BenefitsAr) && !string.IsNullOrWhiteSpace(job.BenefitsEn)
            );

            if (!allTabsCompleted)
                failures.Add(new ValidationFailure("TabsCompletion", JobMessages.AllTabsRequired));
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
            if (job.ClosingDate > DateTime.UtcNow)
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

        return Task.FromResult(failures.Count != 0
            ? new ValidationResult(failures)
            : new ValidationResult());
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

        if (dto.ManagementId != Guid.Empty && dto.DepartmentId != Guid.Empty)
        {
            var isValid = await IsDepartmentUnderManagement(dto.DepartmentId, dto.ManagementId);
            if (!isValid)
                failures.Add(new ValidationFailure(
                    nameof(dto.DepartmentId),
                    JobMessages.DepartmentNotUnderManagement));
        }

        if (dto.MajorId != Guid.Empty && dto.SubMajorId.HasValue)
        {
            var isValid = await IsSubMajorUnderMajor(dto.SubMajorId.Value, dto.MajorId);
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
}
