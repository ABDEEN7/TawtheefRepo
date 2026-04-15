using Application.Operation.Features.Employee.JobManagement.Job.DTOs;
using FluentValidation.Results;
using Tawtheef.Domain.Entities.Recruitment;

namespace Application.Operation.Common.Validations;

public interface IJobValidationService
{
    Task<ValidationResult> ValidateForCreation(CreateJobDto dto);
    Task<ValidationResult> ValidateBasicsUpdate(UpdateJobBasicsDto dto, Job existingJob);
    Task<ValidationResult> ValidateOverviewUpdate(UpdateJobOverviewDto dto, Job existingJob);
    Task<ValidationResult> ValidateQualificationsUpdate(UpdateJobQualificationsDto dto, Job existingJob);
    Task<ValidationResult> ValidateResponsibilitiesUpdate(UpdateJobResponsibilitiesDto dto, Job existingJob);
    Task<ValidationResult> ValidateConditionsUpdate(UpdateJobConditionsDto dto, Job existingJob);
    Task<ValidationResult> ValidateSkillsUpdate(UpdateJobSkillsDto dto, Job existingJob);
    Task<ValidationResult> ValidateAttachmentsUpdate(UpdateJobAttachmentsDto dto, Job existingJob);
    Task<ValidationResult> ValidateBenefitsUpdate(UpdateJobBenefitsDto dto, Job existingJob);
    Task<ValidationResult> ValidateStatusChange(Job job, Guid newStatusId);

    public Task<bool> IsDuplicateJob(Guid? jobId, Guid managementId, Guid sectorId, Guid jobTitleId,
        Guid genderId, Guid? departmentId = null);
}
