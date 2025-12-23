using FluentValidation;
using Tawtheef.Application.Features.Recruitment.Profile.DTOs;

namespace Tawtheef.Application.Features.Recruitment.Profile.Validators;

internal sealed class AchievementUpsertValidator : AbstractValidator<AchievementUpsertDto>
{
    public AchievementUpsertValidator()
    {
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.IssuingAuthority).NotEmpty();
        RuleFor(x => x.CountryId).NotEmpty();
        RuleFor(x => x.AchievementTypeId).NotEmpty();
        RuleFor(x => x.IssueDate).NotNull();
    }
}