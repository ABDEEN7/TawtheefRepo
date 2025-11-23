// UpdateJobCommandValidator.cs
using FluentValidation;
using Tawtheef.Application.Features.Operations.Employee.Job.DTOs;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Commands;

public class UpdateJobCommandValidator : AbstractValidator<UpdateJobCommand>
{
    public UpdateJobCommandValidator()
    {
        RuleFor(x => x.JobId)
            .NotEmpty().WithMessage("Job ID is required");
            
        RuleFor(x => x.Job)
            .NotNull().WithMessage("Job data is required");

        // Basics validation
        RuleFor(x => x.Job.Basics).NotNull().WithMessage("Job basics are required");
        RuleFor(x => x.Job.Basics.Title)
            .NotEmpty().WithMessage("Job title is required")
            .MaximumLength(200).WithMessage("Job title cannot exceed 200 characters");
            
        RuleFor(x => x.Job.Basics.Vacancies)
            .GreaterThan(0).WithMessage("Vacancies must be greater than 0");
            
        RuleFor(x => x.Job.Basics.Deadline)
            .GreaterThan(DateTimeOffset.UtcNow.AddHours(24))
            .WithMessage("Deadline must be at least 24 hours from now");
            
        RuleFor(x => x.Job.Basics.RequestingDeptId)
            .NotEmpty().WithMessage("Requesting department is required");

        // Quotas validation
        RuleFor(x => x.Job.Quotas).NotNull().WithMessage("Job quotas are required");
        RuleFor(x => x.Job.Quotas)
            .Must(HaveValidQuotaTotal).WithMessage("Job quotas must total 100%");
            
        RuleForEach(x => x.Job.Quotas.ResidentsBreakdown)
            .ChildRules(breakdown =>
            {
                breakdown.RuleFor(b => b.Percentage)
                    .GreaterThan(0).WithMessage("Percentage must be greater than 0")
                    .LessThanOrEqualTo(100).WithMessage("Percentage cannot exceed 100");
            });

        // Collections validation
        RuleForEach(x => x.Job.Conditions)
            .NotEmpty().WithMessage("Condition text cannot be empty")
            .MaximumLength(500).WithMessage("Condition cannot exceed 500 characters");
            
        RuleForEach(x => x.Job.Skills)
            .NotEmpty().WithMessage("Skill text cannot be empty")
            .MaximumLength(100).WithMessage("Skill cannot exceed 100 characters");
    }

    private bool HaveValidQuotaTotal(JobQuotasDto? quotas)
    {
        if (quotas == null) return false;
        
        var total = quotas.QatariCitizens + quotas.QatarMother + quotas.NonQatariSpouse + 
                   quotas.Gcc + quotas.QuGrads + quotas.Residents;
        return total == 100m;
    }
}
