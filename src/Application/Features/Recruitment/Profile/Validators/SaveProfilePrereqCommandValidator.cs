using FluentValidation;
using Tawtheef.Application.Features.Recruitment.Profile.Command.SaveOperation;
using Tawtheef.Application.Features.Recruitment.Profile.DTOs;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Application.Features.Recruitment.Profile.Validators;

public sealed class SaveProfilePrereqCommandValidator : AbstractValidator<SaveProfilePrereqCommand>
{
    public SaveProfilePrereqCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();

        RuleFor(x => x.Request)
            .NotNull()
            .DependentRules(() =>
            {
                RuleFor(x => x.Request.CandidateTypeId).NotEmpty();
                RuleFor(x => x.Request.TargetEntityId).NotEmpty();

                When(x => RequiresOfficeSelection(x.Request), () =>
                {
                    RuleFor(x => x.Request.OfficeId)
                        .NotEmpty()
                        .WithMessage(ErrorsCodes.OfficeRequired);
                });

                When(x => RequiresResidencyExpiry(x.Request), () =>
                {
                    RuleFor(x => x.Request.QIDExpiry).NotEmpty();
                });

                RuleFor(x => x.Request.CvFile)
                    .Must((cmd, file) => FileValidationHelpers.HasFile(file) ||
                                         FileValidationHelpers.HasExisting(cmd.Request.CvFileName))
                    .WithMessage(ErrorsCodes.CvFileRequired);

                RuleFor(x => x.Request.IdFile)
                    .Must((cmd, file) => FileValidationHelpers.HasFile(file) ||
                                         FileValidationHelpers.HasExisting(cmd.Request.IdFileName))
                    .WithMessage(ErrorsCodes.IdFileRequired);

                When(x => RequiresMarriageCertificate(x.Request), () =>
                {
                    RuleFor(x => x.Request.MarriageCertificateFile)
                        .Must((cmd, file) => FileValidationHelpers.HasFile(file) ||
                                             HasExistingMarriageFile(cmd.Request))
                        .WithMessage(ErrorsCodes.MarriageCertificateFileRequired);
                });

                When(x => RequiresBirthCertificate(x.Request), () =>
                {
                    RuleFor(x => x.Request.BirthCertificateFile)
                        .Must((cmd, file) => FileValidationHelpers.HasFile(file) ||
                                             FileValidationHelpers.HasExisting(cmd.Request.BirthCertificateFileName))
                        .WithMessage(ErrorsCodes.BirthCertificateFileRequired);
                });
            });
    }

    private static bool RequiresBirthCertificate(SaveProfilePrereqRequest r)
        => r.CandidateTypeId == CandidateTypeIds.SonOfQatariMother;

    private static bool RequiresMarriageCertificate(SaveProfilePrereqRequest r)
        => r.CandidateTypeId == CandidateTypeIds.WifeOfQatari;

    private static bool RequiresOfficeSelection(SaveProfilePrereqRequest r)
        => r.CandidateTypeId == CandidateTypeIds.NonQatari || r.CandidateTypeId == CandidateTypeIds.GCC;

    private static bool RequiresResidencyExpiry(SaveProfilePrereqRequest r)
        => r.CandidateTypeId != CandidateTypeIds.NonQatari && r.CandidateTypeId != CandidateTypeIds.GCC;

    private static bool HasExistingMarriageFile(SaveProfilePrereqRequest r)
        => FileValidationHelpers.HasExisting(r.MarriageCertificateFileName) ||
           FileValidationHelpers.HasExisting(r.MarriageCertFileName);
}