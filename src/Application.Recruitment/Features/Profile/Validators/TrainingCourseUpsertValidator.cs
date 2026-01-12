using Application.Recruitment.Features.Profile.DTOs.SaveOperation;
using FluentValidation;

namespace Application.Recruitment.Features.Profile.Validators;

internal sealed class TrainingCourseUpsertValidator : AbstractValidator<TrainingCourseUpsertDto>
{
    public TrainingCourseUpsertValidator()
    {
        RuleFor(x => x.Provider).NotEmpty();
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.StartDate).NotEmpty();
        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.StartDate)
            .When(x => x.EndDate.HasValue);
    }
}
