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
            failures.Add(new ValidationFailure("BasicFields", JobValidationMessages.FIELD_REQUIRED));
        }

        if (!JobBusinessRules.IsValidVacancyCount(dto.NumberOfVacancies))
            failures.Add(new ValidationFailure(nameof(dto.NumberOfVacancies),
                JobValidationMessages.VACANCIES_GREATER_THAN_ZERO));

        if (!JobBusinessRules.IsValidClosingDate(dto.ClosingDate))
            failures.Add(new ValidationFailure(nameof(dto.ClosingDate),
                JobValidationMessages.CLOSING_DATE_FUTURE));

        var ageValidation = JobBusinessRules.ValidateAgeRange(
            dto.MinimumAge, dto.MaximumAge, 18, 65);

        if (!ageValidation.IsValid)
            failures.Add(new ValidationFailure("AgeRange", ageValidation.ErrorMessage));

        if (await IsDuplicateJob(dto))
            failures.Add(new ValidationFailure("Duplicate", JobValidationMessages.DUPLICATE_JOB));

        var hierarchicalErrors = await ValidateHierarchicalRelationships(dto);
        failures.AddRange(hierarchicalErrors);

        return failures.Count != 0 ? new ValidationResult(failures) : new ValidationResult();
    }

    private async Task<List<ValidationFailure>> ValidateSkillsByMajor(UpdateJobDto dto)
    {
        var failures = new List<ValidationFailure>();

        if (dto.Skills?.Any() == true && dto.MajorId != Guid.Empty)
        {
            var skillRepo = unitOfWork.GetEntityRepository<Skill>();
            List<Skill> skills = await skillRepo.DbSet
                .AsNoTracking()
                .Where(s => s.MajorId == dto.MajorId).ToListAsync();


            var validSkillIds = skills.Select(k => k.Id).ToHashSet();

            var invalidSkillIds = dto.Skills
                .Where(s => !validSkillIds.Contains(s.SkillId))
                .Select(s => s.SkillId)
                .ToList();

            if (invalidSkillIds.Count != 0)
            {
                failures.Add(new ValidationFailure(
                    nameof(dto.Skills),
                    JobValidationMessages.SKILL_NOT_IN_MAJOR));
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
            failures.Add(new ValidationFailure("AgeRange", JobValidationMessages.CANNOT_MODIFY_AGE_RANGE));
        }

        if (!JobBusinessRules.CanEdit(existingJob.JobStatusId))
        {
            failures.Add(new ValidationFailure("EditMode", JobValidationMessages.CAN_ONLY_EDIT_IN_DRAFT));
        }

        if (JobBusinessRules.IsInApprovalProcess(existingJob.JobStatusId))
        {
            failures.Add(new ValidationFailure("ApprovalStatus", JobValidationMessages.CANNOT_EDIT_IN_APPROVAL));
        }

        if (dto.Conditions?.Any(c => string.IsNullOrWhiteSpace(c.TextAr)) == true)
            failures.Add(new ValidationFailure("Condition.TextAr", JobValidationMessages.CONDITION_TEXT_REQUIRED));

        if (dto.Conditions?.Any(c => string.IsNullOrWhiteSpace(c.TextEn)) == true)
            failures.Add(new ValidationFailure("Condition.TextEn", JobValidationMessages.CONDITION_TEXT_REQUIRED));

        if (dto.Responsibilities?.Any(r => string.IsNullOrWhiteSpace(r.TextAr)) == true)
            failures.Add(new ValidationFailure("Responsibility.TextAr", JobValidationMessages.RESPONSIBILITY_TEXT_REQUIRED));

        if (dto.Responsibilities?.Any(r => string.IsNullOrWhiteSpace(r.TextEn)) == true)
            failures.Add(new ValidationFailure("Responsibility.TextEn", JobValidationMessages.RESPONSIBILITY_TEXT_REQUIRED));

        if (dto.Conditions != null && HasDuplicates(dto.Conditions, c => c.TextAr))
            failures.Add(new ValidationFailure("Conditions", JobValidationMessages.DUPLICATE_CONDITION));

        if (dto.Conditions != null && HasDuplicates(dto.Conditions, c => c.TextEn))
            failures.Add(new ValidationFailure("Conditions", JobValidationMessages.DUPLICATE_CONDITION));

        if (dto.Responsibilities != null && HasDuplicates(dto.Responsibilities, r => r.TextAr))
            failures.Add(new ValidationFailure("Responsibilities", JobValidationMessages.DUPLICATE_RESPONSIBILITY));

        if (dto.Responsibilities != null && HasDuplicates(dto.Responsibilities, r => r.TextEn))
            failures.Add(new ValidationFailure("Responsibilities", JobValidationMessages.DUPLICATE_RESPONSIBILITY));

        if (dto.RequiredAttachments != null && HasDuplicates(dto.RequiredAttachments, a => a.TitleAr))
            failures.Add(new ValidationFailure("RequiredAttachments", JobValidationMessages.DUPLICATE_ATTACHMENT));

        if (dto.RequiredAttachments != null && HasDuplicates(dto.RequiredAttachments, a => a.TitleEn))
            failures.Add(new ValidationFailure("RequiredAttachments", JobValidationMessages.DUPLICATE_ATTACHMENT));

        if (dto.RequiredAttachments?.Any(a => string.IsNullOrWhiteSpace(a.TitleAr)) == true)
            failures.Add(new ValidationFailure("Attachment.TitleAr", JobValidationMessages.ATTACHMENT_TITLE_REQUIRED));

        if (dto.RequiredAttachments?.Any(a => string.IsNullOrWhiteSpace(a.TitleEn)) == true)
            failures.Add(new ValidationFailure("Attachment.TitleEn", JobValidationMessages.ATTACHMENT_TITLE_REQUIRED));

        if ((dto.TitleAr != existingJob.TitleAr || dto.TitleEn != existingJob.TitleEn) &&
            !JobBusinessRules.CanModifyTitle(existingJob.JobStatusId))
        {
            failures.Add(new ValidationFailure("Title", JobValidationMessages.CANNOT_MODIFY_TITLE));
        }

        if (HaveDegreesChanged(dto.Degrees, existingJob.JobDegrees) &&
            !JobBusinessRules.CanModifyQualifications(existingJob.JobStatusId))
        {
            failures.Add(new ValidationFailure("Degrees", JobValidationMessages.CANNOT_MODIFY_QUALIFICATIONS));
        }

        if (HaveSkillsChanged(dto.Skills, existingJob.JobSkills) &&
            !JobBusinessRules.CanModifySkills(existingJob.JobStatusId))
        {
            failures.Add(new ValidationFailure("Skills", JobValidationMessages.CANNOT_MODIFY_SKILLS));
        }

        var skillMajorErrors = await ValidateSkillsByMajor(dto);
        failures.AddRange(skillMajorErrors);

        return failures.Count != 0 ? new ValidationResult(failures) : new ValidationResult();
    }

    public Task<ValidationResult> ValidateStatusChange(JobEntity job, Guid newStatusId)
    {
        var failures = new List<ValidationFailure>();

        if (newStatusId == JobStatusIds.PendingApproval && job.JobStatusId == JobStatusIds.Draft)
        {
            var allTabsCompleted = JobBusinessRules.AreAllTabsCompleted(
                job.JobDegrees.Any(),
                job.JobConditions.Any(),
                job.JobResponsibilities.Any(),
                job.JobRequiredAttachments.Any(),
                !string.IsNullOrWhiteSpace(job.OverViewAr) && !string.IsNullOrWhiteSpace(job.OverViewEn),
                !string.IsNullOrWhiteSpace(job.BenefitsAr) && !string.IsNullOrWhiteSpace(job.BenefitsEn)
            );

            if (!allTabsCompleted)
                failures.Add(new ValidationFailure("TabsCompletion", JobValidationMessages.ALL_TABS_REQUIRED));
        }

        if (newStatusId == JobStatusIds.Published &&
            job.JobStatusId != JobStatusIds.Approved)
        {
            failures.Add(new ValidationFailure(
                "Status",
                JobValidationMessages.CAN_ONLY_PUBLISH_APPROVED));
        }

        if (newStatusId == JobStatusIds.Closed)
        {
            if (job.ClosingDate > DateTime.UtcNow)
            {
                failures.Add(new ValidationFailure(
                    nameof(job.ClosingDate),
                    JobValidationMessages.CANNOT_CLOSE_MANUALLY_BEFORE_DEADLINE));
            }
        }

        if (newStatusId == JobStatusIds.Cancelled)
        {
            var finalStates = new[] { JobStatusIds.Closed, JobStatusIds.Cancelled };
            if (finalStates.Contains(job.JobStatusId))
            {
                failures.Add(new ValidationFailure(
                    "Status",
                    JobValidationMessages.CANNOT_CANCEL_FINAL_STATE));
            }
        }

        if (!JobBusinessRules.IsValidStatusTransition(job.JobStatusId, newStatusId))
        {
            failures.Add(new ValidationFailure(
                "StateMachine",
                JobValidationMessages.INVALID_STATUS_TRANSITION));
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
                    JobValidationMessages.MANAGEMENT_NOT_UNDER_SECTOR));
        }

        if (dto.ManagementId != Guid.Empty && dto.DepartmentId != Guid.Empty)
        {
            var isValid = await IsDepartmentUnderManagement(dto.DepartmentId, dto.ManagementId);
            if (!isValid)
                failures.Add(new ValidationFailure(
                    nameof(dto.DepartmentId),
                    JobValidationMessages.DEPARTMENT_NOT_UNDER_MANAGEMENT));
        }

        if (dto.MajorId != Guid.Empty && dto.SubMajorId.HasValue)
        {
            var isValid = await IsSubMajorUnderMajor(dto.SubMajorId.Value, dto.MajorId);
            if (!isValid)
                failures.Add(new ValidationFailure(
                    nameof(dto.SubMajorId),
                    JobValidationMessages.SUBMAJOR_NOT_UNDER_MAJOR));
        }

        return failures;
    }

    private async Task<bool> IsManagementUnderSector(Guid managementId, Guid sectorId)
    {
        var managementRepo = unitOfWork.GetEntityRepository<Managment>();
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
