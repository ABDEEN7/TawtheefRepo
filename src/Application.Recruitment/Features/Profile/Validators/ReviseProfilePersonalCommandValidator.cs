using Application.Recruitment.Features.Profile.Command.RevisionOperation;
using FluentValidation;
using Tawtheef.Domain.Constants;

namespace Application.Recruitment.Features.Profile.Validators;

public sealed class ReviseProfilePersonalCommandValidator : AbstractValidator<ReviseProfilePersonalCommand>
{
    public ReviseProfilePersonalCommandValidator()
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

                When(x => x.Request.SponsorTypeId.HasValue, () => 
                {
                    RuleFor(x => x.Request.SponsorEmployerName).NotEmpty();
                    RuleFor(x => x.Request.SponsorEmployerNumber).NotEmpty();
                    RuleFor(x => x.Request.SponsorCardExpiryData).NotEmpty();

                    RuleFor(x => x.Request)
                        .Must(r =>
                            FileValidationHelpers.HasFile(r.SponsorCard) ||
                            FileValidationHelpers.HasExisting(r.SponsorCardFileName))
                        .WithMessage(ErrorsCodes.SponsorCardRequired);
                });
            });
    }
}
