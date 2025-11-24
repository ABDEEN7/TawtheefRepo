using FluentValidation;
using Tawtheef.Application.Common.Constants;
using Tawtheef.Application.Features.Operations.Employee.Job.DTOs;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Commands;

public class UpdateJobCommandValidator : AbstractValidator<UpdateJobCommand>
{
    public UpdateJobCommandValidator()
    {
        RuleFor(x => x.JobId)
            .NotEmpty().WithMessage(JobValidationMessages.JobIdRequired);

        RuleFor(x => x.Job)
            .NotNull().WithMessage(JobValidationMessages.JobRequired);
        

        RuleFor(x => x.Job.Title)
            .NotEmpty().WithMessage(JobValidationMessages.JobTitleRequired)
            .MaximumLength(200).WithMessage(JobValidationMessages.JobTitleMaxLength);

        RuleFor(x => x.Job.Vacancies)
            .GreaterThan(0).WithMessage(JobValidationMessages.VacanciesGreaterThanZero);

        RuleFor(x => x.Job.Deadline)
            .GreaterThan(DateTimeOffset.UtcNow.AddHours(24))
            .WithMessage(JobValidationMessages.DeadlineAtLeast24Hours);

        RuleFor(x => x.Job.RequestingDepartmentId)
            .NotEmpty().WithMessage(JobValidationMessages.RequestingDeptRequired);

        // Quotas validation
        RuleFor(x => x.Job.Quota)
            .NotNull().WithMessage(JobValidationMessages.JobQuotasRequired);

        RuleFor(x => x.Job.Quota)
            .Must(HaveValidQuotaTotal)
            .WithMessage(JobValidationMessages.JobQuotaTotalInvalid);

        RuleForEach(x => x.Job.Quota.ResidentsBreakdown)
            .ChildRules(breakdown =>
            {
                breakdown.RuleFor(b => b.Percentage)
                    .GreaterThan(0).WithMessage(JobValidationMessages.QuotaPercentageGreaterThanZero)
                    .LessThanOrEqualTo(100).WithMessage(JobValidationMessages.QuotaPercentageMax100);
            });

        RuleForEach(x => x.Job.Conditions)
            .NotEmpty().WithMessage(JobValidationMessages.ConditionRequired)
            .MaximumLength(500).WithMessage(JobValidationMessages.ConditionMaxLength);

        RuleForEach(x => x.Job.Skills)
            .NotEmpty().WithMessage(JobValidationMessages.SkillRequired)
            .MaximumLength(100).WithMessage(JobValidationMessages.SkillMaxLength);
    }

    private bool HaveValidQuotaTotal(JobQuotaDto? quotas)
    {
        if (quotas == null) return false;

        var total = quotas.QatariCitizens + quotas.QatarMother + quotas.NonQatariSpouse +
                    quotas.Gcc + quotas.QuGrads + quotas.Residents;

        return total == 100m;
    }
}
