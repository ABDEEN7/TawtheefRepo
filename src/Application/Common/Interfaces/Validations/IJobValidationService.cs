using FluentValidation.Results;
using Tawtheef.Application.Features.Operations.Employee.Job.Dtos;
using Tawtheef.Application.Features.Operations.Employee.Job.DTOs;
using Tawtheef.Domain.Entities.Recruitment;

namespace Tawtheef.Application.Common.Interfaces.Validations;

public interface IJobValidationService
{
    Task<ValidationResult> ValidateForCreation(CreateJobDto dto);
    Task<ValidationResult> ValidateForUpdate(UpdateJobDto dto, Job existingJob);
    Task<ValidationResult> ValidateStatusChange(Job job, Guid newStatusId);
}
