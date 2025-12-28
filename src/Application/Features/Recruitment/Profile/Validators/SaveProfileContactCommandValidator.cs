using FluentValidation;
using Tawtheef.Application.Features.Recruitment.Profile.Command.SaveOperation;
using Tawtheef.Application.Features.Recruitment.Profile.DTOs;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Application.Features.Recruitment.Profile.Validators;

public sealed class SaveProfileContactCommandValidator : AbstractValidator<SaveProfileContactCommand>
{
    public SaveProfileContactCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();

        RuleFor(x => x.Request)
            .NotNull()
            .DependentRules(() =>
            {
                RuleFor(x => x.Request.ResidenceCountryId).NotEmpty();
                RuleFor(x => x.Request.InterviewLocationId).NotEmpty();

                RuleFor(x => x.Request)
                    .Must(r => !string.IsNullOrWhiteSpace(r.Address) ||
                               r.NationalAddress is not null)
                    .WithMessage(ErrorsCodes.AddressRequired);
                
                When(x => x.Request.NationalAddress is not null, () =>
                {
                    RuleFor(x => x.Request.NationalAddress!.Zone).GreaterThan(0);
                    RuleFor(x => x.Request.NationalAddress!.Street).GreaterThan(0);
                    RuleFor(x => x.Request.NationalAddress!.Building).GreaterThan(0);

                    RuleFor(x => x.Request.NationalAddress!)
                        .Must(na =>
                            FileValidationHelpers.HasFile(na.NationalAddress) ||
                            FileValidationHelpers.HasExisting(na.NationalAddressFileName))
                        .WithMessage(ErrorsCodes.NationalAddressDocumentRequired);
                });
            });
    }
}
