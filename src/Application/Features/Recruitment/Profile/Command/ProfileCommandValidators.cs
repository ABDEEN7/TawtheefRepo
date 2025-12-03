using FluentValidation;
using Microsoft.AspNetCore.Http;
using Tawtheef.Application.Features.Recruitment.Profile.DTOs;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Application.Features.Recruitment.Profile.Command;

#region Shared Helpers

internal static class FileValidationHelpers
{
    public static bool HasFile(IFormFile? file) => file is { Length: > 0 };
    public static bool HasExisting(string? fileName) => !string.IsNullOrWhiteSpace(fileName);
}

#endregion

#region Profile Prereq

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

    private static bool HasExistingMarriageFile(SaveProfilePrereqRequest r)
        => FileValidationHelpers.HasExisting(r.MarriageCertificateFileName) ||
           FileValidationHelpers.HasExisting(r.MarriageCertFileName);
}

#endregion

#region Profile Personal

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
                
                    RuleFor(x => x.Request.SponsorCard)
                        .Cascade(CascadeMode.Stop)
                        .NotNull().WithMessage(ErrorsCodes.SponsorCardRequired)
                        .Must(FileValidationHelpers.HasFile)
                        .WithMessage(ErrorsCodes.SponsorCardFileRequired);
                });
            });
    }
}

#endregion

#region Profile Contact

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
                    
                    RuleFor(x => x.Request.NationalAddress!.NationalAddress)
                        .Cascade(CascadeMode.Stop)
                        .NotNull().WithMessage(ErrorsCodes.NationalAddressRequired)
                        .Must(FileValidationHelpers.HasFile)
                        .WithMessage(ErrorsCodes.NationalAddressDocumentRequired);
                });
            });
    }
}

#endregion

#region Profile Education

public sealed class SaveProfileEducationCommandValidator : AbstractValidator<SaveProfileEducationCommand>
{
    public SaveProfileEducationCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();

        RuleFor(x => x.Request)
            .NotNull()
            .DependentRules(() =>
            {
                RuleFor(x => x.Request.DegreesJson).NotEmpty();

                RuleFor(x => x.Request.DegreeFiles)
                    .Cascade(CascadeMode.Stop)
                    .NotNull().WithMessage(ErrorsCodes.DegreeFileRequired)
                    .Must(files => files!.Count > 0 && files.All(FileValidationHelpers.HasFile))
                    .WithMessage(ErrorsCodes.DegreeFileRequired);
            });
    }
}

#endregion

#region Profile Experience

public sealed class SaveProfileExperienceCommandValidator : AbstractValidator<SaveProfileExperienceCommand>
{
    public SaveProfileExperienceCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();

        RuleFor(x => x.Request)
            .NotNull()
            .DependentRules(() =>
            {
                RuleFor(x => x.Request.ExperiencesJson)
                    .NotEmpty()
                    .WithMessage(ErrorsCodes.ExperienceRequired);

                RuleFor(x => x.Request.TrainingCoursesJson).NotNull();
                RuleFor(x => x.Request.ExperienceFiles).NotNull();
                RuleFor(x => x.Request.TrainingCourseFiles).NotNull();
            });
    }
}

#endregion

#region Profile Skills

public sealed class SaveProfileSkillsCommandValidator : AbstractValidator<SaveProfileSkillsCommand>
{
    public SaveProfileSkillsCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();

        RuleFor(x => x.Request)
            .NotNull()
            .DependentRules(() =>
            {
                RuleForEach(x => x.Request.Skills)
                    .ChildRules(skill =>
                    {
                        skill.RuleFor(s => s.SkillId).NotEmpty();
                        skill.RuleFor(s => s.LevelId).NotEmpty();
                    });

                RuleFor(x => x.Request)
                    .Must(r => (r.Skills.Count) > 0)
                    .WithMessage(ErrorsCodes.SkillOrLanguageRequired);
            });
    }
}

#endregion

#region Profile Languages

public sealed class SaveProfileLanguagesCommandValidator : AbstractValidator<SaveProfileLanguagesCommand>
{
    public SaveProfileLanguagesCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();

        RuleFor(x => x.Request)
            .NotNull()
            .DependentRules(() =>
            {
                RuleForEach(x => x.Request.Languages)
                    .ChildRules(lang =>
                    {
                        lang.RuleFor(l => l.LanguageId).NotEmpty();
                        lang.RuleFor(l => l.LevelId).NotEmpty();
                    });

                RuleFor(x => x.Request)
                    .Must(r => (r.Languages.Count) > 0)
                    .WithMessage(ErrorsCodes.SkillOrLanguageRequired);
            });
    }
}

#endregion

#region Profile Attachments

public sealed class SaveProfileAttachmentsCommandValidator : AbstractValidator<SaveProfileAttachmentsCommand>
{
    public SaveProfileAttachmentsCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Request).NotNull()
            .DependentRules(() =>
            {
                RuleFor(x => x.Request.AttachmentsJson).NotEmpty();
                RuleFor(x => x.Request.AttachmentFiles).NotNull();
            });
    }
}

#endregion

#region Submit Profile

public sealed class SubmitUserProfileCommandValidator : AbstractValidator<SubmitUserProfileCommand>
{
    public SubmitUserProfileCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Request).NotNull();
    }
}

#endregion

#region DTO Child Validators

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

#endregion
