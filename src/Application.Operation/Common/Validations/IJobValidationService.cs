using Application.Operation.Features.Employee.Job.DTOs;
using FluentValidation.Results;
using Tawtheef.Domain.Entities.Recruitment;

namespace Application.Operation.Common.Validations;

public interface IJobValidationService
{
    Task<ValidationResult> ValidateForCreation(CreateJobDto dto);
    Task<ValidationResult> ValidateForUpdate(UpdateJobDto dto, Job existingJob);
    Task<ValidationResult> ValidateStatusChange(Job job, Guid newStatusId);
}
