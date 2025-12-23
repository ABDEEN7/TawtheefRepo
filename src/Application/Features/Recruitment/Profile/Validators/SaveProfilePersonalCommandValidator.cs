using FluentValidation;
using Tawtheef.Application.Features.Recruitment.Profile.Command.SaveOperation;
using Tawtheef.Domain.Constants;

namespace Tawtheef.Application.Features.Recruitment.Profile.Validators;

public sealed class SaveProfilePersonalCommandValidator : AbstractValidator<SaveProfilePersonalCommand>
{
    public SaveProfilePersonalCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();

        RuleFor(x => x.Request)
            .NotNull()
            .DependentRules(() =>
            {
                RuleFor(x => x.Request.FullNameAr).NotEmpty();
                RuleFor(x => x.Request.FullNameEn).NotEmpty();
                RuleFor(x => x.Request.NationalNumber).NotEmpty();
                RuleFor(x => x.Request.BirthDate).NotNull();

                RuleFor(x => x.Request.NationalityId).NotNull();
                RuleFor(x => x.Request.GenderId).NotNull();
                RuleFor(x => x.Request.ReligionId).NotNull();
                RuleFor(x => x.Request.MaritalStatusId).NotNull();

                RuleFor(x => x.Request.ChildrenCount)
                    .GreaterThanOrEqualTo(0)
                    .When(x => x.Request.ChildrenCount.HasValue);

                When(x => x.Request.HasDisability, () =>
                {
                    RuleFor(x => x.Request.DisabilityDetails).NotEmpty();
                });

                When(x => x.Request.SponsorTypeId.HasValue, () => 
                {
                    RuleFor(x => x.Request.SponsorEmployerName).NotEmpty();
                    RuleFor(x => x.Request.SponsorEmployerNumber).NotEmpty();
                    RuleFor(x => x.Request.QIDExpiry).NotEmpty();

                    RuleFor(x => x.Request)
                        .Must(r =>
                            FileValidationHelpers.HasFile(r.SponsorCard) ||
                            FileValidationHelpers.HasExisting(r.SponsorCardFileName))
                        .WithMessage(ErrorsCodes.SponsorCardRequired);
                });
            });
    }
}