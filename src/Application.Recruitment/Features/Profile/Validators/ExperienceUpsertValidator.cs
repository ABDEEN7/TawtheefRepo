using Application.Recruitment.Features.Profile.DTOs.SaveOperation;
using FluentValidation;

namespace Application.Recruitment.Features.Profile.Validators;

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
