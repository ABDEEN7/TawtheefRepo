using FluentValidation;
using Microsoft.AspNetCore.Http;
using Tawtheef.Application.Features.Recruitment.Profile.DTOs;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Application.Features.Recruitment.Profile.Command;

public sealed class SaveProfilePrereqCommandValidator : AbstractValidator<SaveProfilePrereqCommand>
{
    public SaveProfilePrereqCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Request).NotNull();

        When(x => x.Request is not null, () =>
        {
            RuleFor(x => x.Request.CandidateTypeId).NotEmpty();
            RuleFor(x => x.Request.TargetEntityId).NotEmpty();

            When(x => x.Request!.Submit, () =>
            {
                RuleFor(x => x.Request!.CvFile)
                    .Must((cmd, file) => HasFile(file) || HasExisting(cmd.Request!.CvFileName))
                    .WithMessage(ErrorsCodes.CvFileRequired);

                RuleFor(x => x.Request!.IdFile)
                    .Must((cmd, file) => HasFile(file) || HasExisting(cmd.Request!.IdFileName))
                    .WithMessage(ErrorsCodes.IdFileRequired);

                When(x => RequiresMarriageCertificate(x.Request!), () =>
                {
                    RuleFor(x => x.Request!.MarriageCertificateFile)
                        .Must((cmd, file) => HasFile(file) || HasExistingMarriageFile(cmd.Request!))
                        .WithMessage(ErrorsCodes.MarriageCertificateFileRequired);
                });

                When(x => RequiresBirthCertificate(x.Request!), () =>
                {
                    RuleFor(x => x.Request!.BirthCertificateFile)
                        .Must((cmd, file) => HasFile(file) || HasExisting(cmd.Request!.BirthCertificateFileName))
                        .WithMessage(ErrorsCodes.BirthCertificateFileRequired);
                });
            });
        });
    }

    private static bool HasFile(IFormFile? file) => file is { Length: > 0 };

    private static bool HasExisting(string? fileName) => !string.IsNullOrWhiteSpace(fileName);

    private static bool RequiresBirthCertificate(SaveProfilePrereqRequest request) =>
        request.CandidateTypeId == CandidateTypeIds.SonOfQatariMother;

    private static bool RequiresMarriageCertificate(SaveProfilePrereqRequest request) =>
        request.CandidateTypeId == CandidateTypeIds.WifeOfQatari;

    private static bool HasExistingMarriageFile(SaveProfilePrereqRequest request) =>
        HasExisting(request.MarriageCertificateFileName) || HasExisting(request.MarriageCertFileName);
}

public sealed class SaveProfilePersonalCommandValidator : AbstractValidator<SaveProfilePersonalCommand>
{
    public SaveProfilePersonalCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Request).NotNull();

        When(x => x.Request is not null, () =>
        {
            RuleFor(x => x.Request!.FullNameAr).NotEmpty();
            RuleFor(x => x.Request!.FullNameEn).NotEmpty();
            RuleFor(x => x.Request!.NationalNumber).NotEmpty();
            RuleFor(x => x.Request!.BirthDate).NotNull();

            RuleFor(x => x.Request!.NationalityId).NotNull();
            RuleFor(x => x.Request!.GenderId).NotNull();
            RuleFor(x => x.Request!.ReligionId).NotNull();
            RuleFor(x => x.Request!.MaritalStatusId).NotNull();
            RuleFor(x => x.Request!.ChildrenCount)
                .GreaterThanOrEqualTo(0)
                .When(x => x.Request!.ChildrenCount.HasValue);

            When(x => x.Request!.HasDisability, () =>
            {
                RuleFor(x => x.Request!.DisabilityDetails).NotEmpty();
            });

            RuleFor(x => x.Request!.SponsorTypeId).NotNull();
            RuleFor(x => x.Request!.SponsorEmployerName).NotEmpty();
            RuleFor(x => x.Request!.SponsorEmployerNumber).NotEmpty();

            When(x => x.Request!.Submit, () =>
            {
                RuleFor(x => x.Request!.SponsorCard)
                    .Must(HasFile)
                    .WithMessage(ErrorsCodes.SponsorCardRequired);
            });

            When(x => !string.IsNullOrWhiteSpace(x.Request!.SponsorEmployerName)
                      || !string.IsNullOrWhiteSpace(x.Request!.SponsorEmployerNumber), () =>
            {
                RuleFor(x => x.Request!.SponsorCard)
                    .Must(file => file is null || HasFile(file))
                    .WithMessage(ErrorsCodes.SponsorCardFileRequired);
            });
        });
    }

    private static bool HasFile(IFormFile? file) => file is { Length: > 0 };
}

public sealed class SaveProfileContactCommandValidator : AbstractValidator<SaveProfileContactCommand>
{
    public SaveProfileContactCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Request).NotNull();

        When(x => x.Request is not null, () =>
        {
            RuleFor(x => x.Request!.ResidenceCountryId).NotEmpty();
            RuleFor(x => x.Request!.InterviewLocationId).NotEmpty();
            RuleFor(x => x.Request!.Address).NotEmpty();

            When(x => x.Request!.Submit, () =>
            {
                RuleFor(x => x.Request!.NationalAddress).NotNull();

                When(x => x.Request!.NationalAddress is not null, () =>
                {
                    RuleFor(x => x.Request!.NationalAddress!.Zone).GreaterThan(0);
                    RuleFor(x => x.Request!.NationalAddress!.Street).GreaterThan(0);
                    RuleFor(x => x.Request!.NationalAddress!.Building).GreaterThan(0);
                    RuleFor(x => x.Request!.NationalAddress!.Unit).GreaterThan(0);
                    RuleFor(x => x.Request!.NationalAddress!.NationalAddress)
                        .Must(HasFile)
                        .WithMessage(ErrorsCodes.NationalAddressDocumentRequired);
                });
            });
        });
    }

    private static bool HasFile(IFormFile? file) => file is { Length: > 0 };
}

public sealed class SaveProfileEducationCommandValidator : AbstractValidator<SaveProfileEducationCommand>
{
    public SaveProfileEducationCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Request).NotNull();

        When(x => x.Request is not null, () =>
        {
            RuleFor(x => x.Request!.DegreesJson).NotEmpty();
            RuleFor(x => x.Request!.DegreeFiles).NotNull();

            When(x => x.Request!.Submit, () =>
            {
                RuleFor(x => x.Request!.DegreeFiles)
                    .Must(files => files is { Count: > 0 } && files.All(HasFile))
                    .WithMessage(ErrorsCodes.DegreeFileRequired);
            });
        });
    }

    private static bool HasFile(IFormFile? file) => file is { Length: > 0 };
}

public sealed class SaveProfileExperienceCommandValidator : AbstractValidator<SaveProfileExperienceCommand>
{
    public SaveProfileExperienceCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Request).NotNull();

        When(x => x.Request is not null, () =>
        {
            RuleFor(x => x.Request!.ExperiencesJson).NotEmpty();
            RuleFor(x => x.Request!.TrainingCoursesJson).NotNull();
            RuleFor(x => x.Request!.ExperienceFiles).NotNull();
            RuleFor(x => x.Request!.TrainingCourseFiles).NotNull();

            When(x => x.Request!.Submit, () =>
            {
                RuleFor(x => x.Request!.ExperiencesJson)
                    .NotEmpty()
                    .WithMessage(ErrorsCodes.ExperienceRequired);
            });
        });
    }
}

public sealed class SaveProfileSkillsCommandValidator : AbstractValidator<SaveProfileSkillsCommand>
{
    public SaveProfileSkillsCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Request).NotNull();

        When(x => x.Request is not null, () =>
        {
            RuleForEach(x => x.Request!.Skills).ChildRules(skill =>
            {
                skill.RuleFor(s => s.SkillId).NotEmpty();
                skill.RuleFor(s => s.LevelId).NotEmpty();
            });
            When(x => x.Request!.Submit, () =>
            {
                RuleFor(x => x.Request!)
                    .Must(r => (r.Skills?.Count ?? 0) > 0)
                    .WithMessage(ErrorsCodes.SkillOrLanguageRequired);
            });
        });
    }
}
public sealed class SaveProfileLanguagesCommandValidator : AbstractValidator<SaveProfileLanguagesCommand>
{
    public SaveProfileLanguagesCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Request).NotNull();

        When(x => x.Request is not null, () =>
        {
            RuleForEach(x => x.Request!.Languages).ChildRules(lang =>
            {
                lang.RuleFor(l => l.LanguageId).NotEmpty();
                lang.RuleFor(l => l.LevelId).NotEmpty();
            });

            When(x => x.Request!.Submit, () =>
            {
                RuleFor(x => x.Request!)
                    .Must(r => (r.Languages?.Count ?? 0) > 0)
                    .WithMessage(ErrorsCodes.SkillOrLanguageRequired);
            });
        });
    }
}

public sealed class SaveProfileAttachmentsCommandValidator : AbstractValidator<SaveProfileAttachmentsCommand>
{
    public SaveProfileAttachmentsCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Request).NotNull();

        When(x => x.Request is not null, () =>
        {
            RuleFor(x => x.Request!.AttachmentsJson).NotEmpty();
            RuleFor(x => x.Request!.AttachmentFiles).NotNull();
        });
    }
}

public sealed class SubmitUserProfileCommandValidator : AbstractValidator<SubmitUserProfileCommand>
{
    public SubmitUserProfileCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Request).NotNull();
    }
}

internal sealed class ExperienceUpsertValidator : AbstractValidator<ExperienceUpsertDto>
{
    public ExperienceUpsertValidator()
    {
        RuleFor(x => x.Organization).NotEmpty();
        RuleFor(x => x.Position).NotEmpty();
        RuleFor(x => x.StartDate).NotEmpty();
        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.StartDate)
            .When(x => x.EndDate.HasValue);
    }
}

internal sealed class TrainingCourseUpsertValidator : AbstractValidator<TrainingCourseUpsertDto>
{
    public TrainingCourseUpsertValidator()
    {
        RuleFor(x => x.Organization).NotEmpty();
        RuleFor(x => x.Position).NotEmpty();
        RuleFor(x => x.StartDate).NotEmpty();
        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.StartDate)
            .When(x => x.EndDate.HasValue);
    }
}
