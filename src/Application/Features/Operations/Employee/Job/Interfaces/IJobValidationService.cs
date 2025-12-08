using FluentValidation.Results;
using Tawtheef.Application.Features.Operations.Employee.Job.Dtos;
using Tawtheef.Application.Features.Operations.Employee.Job.DTOs;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Interfaces;

public interface IJobValidationService
{
    Task<ValidationResult> ValidateForCreation(CreateJobDto dto);
    Task<ValidationResult> ValidateForUpdate(UpdateJobDto dto, Domain.Entities.Recruitment.Job existingJob);
    Task<ValidationResult> ValidateStatusChange(Domain.Entities.Recruitment.Job job, Guid newStatusId);
}
