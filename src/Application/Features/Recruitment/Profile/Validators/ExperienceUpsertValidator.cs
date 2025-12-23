using FluentValidation;
using Tawtheef.Application.Features.Recruitment.Profile.DTOs;

namespace Tawtheef.Application.Features.Recruitment.Profile.Validators;

internal sealed class ExperienceUpsertValidator : AbstractValidator<ExperienceUpsertDto>
{
    public ExperienceUpsertValidator()
    {
        RuleFor(x => x.EmployerName).NotEmpty();
        RuleFor(x => x.JobTitle).NotEmpty();
        RuleFor(x => x.StartDate).NotEmpty();
        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.StartDate)
            .When(x => x.EndDate.HasValue);
    }
}