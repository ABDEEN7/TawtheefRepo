using FluentValidation;
using Tawtheef.Application.Common.Constants;
using Tawtheef.Application.Features.Operations.Employee.Job.DTOs;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Commands;

public class CreateJobCommandValidator : AbstractValidator<CreateJobCommand>
{
    public CreateJobCommandValidator()
    {
        RuleFor(x => x.Job)
            .NotNull().WithMessage(JobValidationMessages.JOB_REQUIRED);

        RuleFor(x => x.Job.Title)
            .NotEmpty().WithMessage(JobValidationMessages.JOB_TITLE_REQUIRED)
            .MaximumLength(200).WithMessage(JobValidationMessages.JOB_TITLE_MAX_LENGTH);
            
        RuleFor(x => x.Job.Vacancies)
            .GreaterThan(0).WithMessage(JobValidationMessages.VACANCIES_GREATER_THAN_ZERO);
            
        RuleFor(x => x.Job.Deadline)
            .GreaterThan(DateTimeOffset.UtcNow.AddHours(24))
            .WithMessage(JobValidationMessages.DEADLINE_AT_LEAST_24_HOURS);
            
        RuleFor(x => x.Job.RequestingDepartmentId)
            .NotEmpty().WithMessage(JobValidationMessages.REQUESTING_DEPT_REQUIRED);

        RuleFor(x => x.Job.Quota)
            .NotNull().WithMessage(JobValidationMessages.JOB_QUOTAS_REQUIRED);

        RuleFor(x => x.Job.Quota)
            .Must(HaveValidQuotaTotal)
            .WithMessage(JobValidationMessages.JOB_QUOTA_TOTAL_INVALID);
            
        RuleForEach(x => x.Job.Quota.ResidentsBreakdowns)
            .ChildRules(breakdown =>
            {
                breakdown.RuleFor(b => b.Percentage)
                    .GreaterThan(0).WithMessage(JobValidationMessages.QUOTA_PERCENTAGE_GREATER_THAN_ZERO)
                    .LessThanOrEqualTo(100).WithMessage(JobValidationMessages.QUOTA_PERCENTAGE_MAX_100);
            });

        RuleForEach(x => x.Job.Conditions)
            .NotEmpty().WithMessage(JobValidationMessages.CONDITION_REQUIRED)
            .MaximumLength(500).WithMessage(JobValidationMessages.CONDITION_MAX_LENGTH);
            
        RuleForEach(x => x.Job.Skills)
            .NotEmpty().WithMessage(JobValidationMessages.SKILL_REQUIRED)
            .MaximumLength(100).WithMessage(JobValidationMessages.SKILL_MAX_LENGTH);
    }

    private bool HaveValidQuotaTotal(JobQuotaDto? quotas)
    {
        if (quotas == null) return false;
        
        var total = quotas.QatariCitizens + quotas.QatarMother + quotas.NonQatariSpouse + 
                   quotas.Gcc + quotas.QuGrads + quotas.Residents;
        return total == 100m;
    }
}
