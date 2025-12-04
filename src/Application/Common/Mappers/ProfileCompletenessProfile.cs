using Mapster;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;
using Tawtheef.Domain.Entities;
using Tawtheef.Domain.Entities.Applicant;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Common.Mappers;

public sealed class ProfileCompletenessProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Resource, FileRefDto>()
            .Map(dest => dest.ResourceId, src => src.Id)
            .Map(dest => dest.FileName, src => src.Name);

        config.NewConfig<ProfileAdditionalAttachment, AdditionalAttachmentDto>()
            .Map(dest => dest.Title, src => src.FileName)
            .Map(dest => dest.File, src => src.Attachment);

        config.NewConfig<Qualification, QualificationDto>()
            .Map(dest => dest.GradCountryId, src => src.CountryId)
            .Map(dest => dest.Gpa, src => src.GPA)
            .Map(dest => dest.GradeId, src => src.RatingId)
            .Map(dest => dest.Attachment, src => src.Certificate);

        config.NewConfig<Experience, ExperienceDto>()
            .Map(dest => dest.Attachment, src => src.Certificate)
            .Map(dest => dest.IsCurrent, src => src.EndDate == null);

        config.NewConfig<TrainingCourse, TrainingCourseDto>()
            .Map(dest => dest.Attachment, src => src.Certificate);

        config.NewConfig<ProfileSkill, SkillDto>()
            .Map(dest => dest.Skill, src => src.Skill);

        config.NewConfig<ProfileLanguage, LanguageDto>()
            .Map(dest => dest.Language, src => src.Language);

        config.NewConfig<(UserProfile profile, User user, ProfilePrefillDto prefill), ProfileStatusDto>()
            .Map(dest => dest, src => src.profile)
            .Map(dest => dest.IsComplete, src => src.profile.IsCompleted())
            .Map(dest => dest.IsDraft, src => src.profile.IsDraft)
            .Map(dest => dest.Avatar, src => src.user.Avatar ?? src.prefill.Avatar)
            .Map(dest => dest.FullNameAr,
                src => string.IsNullOrWhiteSpace(src.user.FullNameAr)
                    ? src.prefill.FullName
                    : src.user.FullNameAr)
            .Map(dest => dest.FullNameEn,
                src => string.IsNullOrWhiteSpace(src.user.FullNameEn)
                    ? src.prefill.FullName
                    : src.user.FullNameEn)
            .Map(dest => dest.Email, src => src.user.Email ?? src.prefill.Email)
            .Map(dest => dest.EmailVerified, src => src.user.EmailConfirmed)
            .Map(dest => dest.Phone, src => src.user.PhoneNumber ?? src.prefill.Phone)
            .Map(dest => dest.PhoneVerified, src => src.user.PhoneNumberConfirmed)
            .Map(dest => dest.NationalNumber, src => src.profile.NationalNumber ?? src.prefill.Qid)
            .Map(dest => dest.QIDExpiry, src => src.profile.QIDExpiry)
            .Map(dest => dest.SponsorTypeId, src => src.profile.SponsorProfile == null ? null : (Guid?)src.profile.SponsorProfile.SponsorTypeId)
            .Map(dest => dest.SponsorEmployerName, src => src.profile.SponsorProfile == null ? null : src.profile.SponsorProfile.SponsorName)
            .Map(dest => dest.SponsorEmployerNumber, src => src.profile.SponsorProfile == null ? null : src.profile.SponsorProfile.SponsorNumber)
            .Map(dest => dest.SponsorCard, src => src.profile.SponsorProfile == null ? null : src.profile.SponsorProfile.SponsorCard)
            .Map(dest => dest.OfficeId, src => src.profile.OfficeId)
            .Map(dest => dest.naZone, src => src.profile.ResidenceAddress == null ? null : (int?)src.profile.ResidenceAddress.ZoneNo)
            .Map(dest => dest.naStreet, src => src.profile.ResidenceAddress == null ? null : (int?)src.profile.ResidenceAddress.StreetNo)
            .Map(dest => dest.naBuilding, src => src.profile.ResidenceAddress == null ? null : (int?)src.profile.ResidenceAddress.BuildingNo)
            .Map(dest => dest.naUnit, src => src.profile.ResidenceAddress == null ? null : (int?)src.profile.ResidenceAddress.UnitNo)
            .Map(dest => dest.ResidenceAddressCertificate, src => src.profile.SponsorProfile == null ? null : src.profile.ResidenceAddressCertificate)
            .Map(dest => dest.AdditionalAttachments,
                 src => 
                     src.profile.AdditionalAttachments == null ? null :
                     src.profile.AdditionalAttachments.Where(a => a.Attachment != null))
            .Map(dest => dest.Qualifications, src => src.profile.Qualifications)
            .Map(dest => dest.Experiences, src => src.profile.Experiences)
            .Map(dest => dest.TrainingCourses, src => src.profile.TrainingCourses)
            .Map(dest => dest.Skills, src => src.profile.Skills)
            .Map(dest => dest.Languages, src => src.profile.Languages)
            ;
    }
}
