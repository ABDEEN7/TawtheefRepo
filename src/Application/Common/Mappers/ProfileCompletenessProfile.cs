using Mapster;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;
using Tawtheef.Domain.Entities.Applicant;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Common.Mappers;

public sealed class ProfileCompletenessProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<ProfileAdditionalAttachment, AdditionalAttachmentDto>()
            .Map(dest => dest.Title, src => src.FileName)
            .Map(dest => dest.File, src => src.Attachment);

        config.NewConfig<Qualification, QualificationDto>()
            .Map(dest => dest.GradCountryId, src => src.CountryId)
            .Map(dest => dest.GradCountry, src => src.Country)
            .Map(dest => dest.Gpa, src => src.GPA)
            .Map(dest => dest.GradeId, src => src.RatingId)
            .Map(dest => dest.Grade, src => src.Rating)
            .Map(dest => dest.Attachment, src => src.Certificate);
        
        config.NewConfig<AchievementDto, Achievement>();

        config.NewConfig<Experience, ExperienceDto>()
            .Map(dest => dest.Attachment, src => src.Certificate)
            .Map(dest => dest.QualificationId, src => src.QualificationId)
            .Map(dest => dest.DegreeName,src=> src.Qualification == null ? null : src.Qualification.Degree)
            .Map(dest => dest.MajorName,src=> src.Qualification == null ? null : src.Qualification.Major)
            .Map(dest => dest.UniversityName,src=> src.Qualification == null ? null : src.Qualification.University)
            .Map(dest => dest.IsCurrent, src => src.EndDate == null);

        config.NewConfig<TrainingCourse, TrainingCourseDto>()
            .Map(dest => dest.Attachment, src => src.Certificate);

        config.NewConfig<Achievement, AchievementDto>();

        config.NewConfig<ProfileSkill, SkillDto>();

        config.NewConfig<ProfileLanguage, LanguageDto>();

        config.NewConfig<ProfileBootstrapSource, ProfileStatusDto>()
            .Map(dest => dest, src => src.Profile)
            .Map(dest => dest.IsComplete,
                src => src.Profile.IsCompleted() && src.Profile.Status != UserProfileStatus.InCreation)
            .Map(dest => dest.Status, src => src.Profile.Status)
            .Map(dest => dest.Avatar, src => src.User.Avatar ?? src.Prefill.Avatar)
            .Map(dest => dest.FullNameAr,
                src => string.IsNullOrWhiteSpace(src.User.FullNameAr)
                    ? src.Prefill.FullName : src.User.FullNameAr)
            .Map(dest => dest.FullNameEn,
                src => string.IsNullOrWhiteSpace(src.User.FullNameEn)
                    ? src.Prefill.FullName : src.User.FullNameEn)
            .Map(dest => dest.Email, src => src.User.Email ?? src.Prefill.Email)
            .Map(dest => dest.EmailVerified, src => src.User.EmailConfirmed)
            .Map(dest => dest.Phone, src => src.User.PhoneNumber ?? src.Prefill.Phone)
            .Map(dest => dest.PhoneVerified, src => src.User.PhoneNumberConfirmed)
            .Map(dest => dest.NationalNumber, src => src.Profile.NationalNumber ?? src.Prefill.Qid)
            .Map(dest => dest.QIDExpiry, src => src.Profile.QIDExpiry)
            .Map(dest => dest.SponsorTypeId, src => src.Profile.SponsorProfile == null ? null : (Guid?)src.Profile.SponsorProfile.SponsorTypeId)
            .Map(dest => dest.SponsorEmployerName, src => src.Profile.SponsorProfile == null ? null : src.Profile.SponsorProfile.SponsorName)
            .Map(dest => dest.SponsorEmployerNumber, src => src.Profile.SponsorProfile == null ? null : src.Profile.SponsorProfile.SponsorNumber)
            .Map(dest => dest.SponsorCard, src => src.Profile.SponsorProfile == null ? null : src.Profile.SponsorProfile.SponsorCard)
            .Map(dest => dest.SponsorQidExpiry, src => src.Profile.SponsorProfile == null ? (DateOnly?)null : src.Profile.SponsorProfile.QIDExpiry)
            .Map(dest => dest.OfficeId, src => src.Profile.OfficeId)
            .Map(dest => dest.naZone, src => src.Profile.ResidenceAddress == null ? null : (int?)src.Profile.ResidenceAddress.ZoneNo)
            .Map(dest => dest.naStreet, src => src.Profile.ResidenceAddress == null ? null : (int?)src.Profile.ResidenceAddress.StreetNo)
            .Map(dest => dest.naBuilding, src => src.Profile.ResidenceAddress == null ? null : (int?)src.Profile.ResidenceAddress.BuildingNo)
            .Map(dest => dest.naUnit, src => src.Profile.ResidenceAddress == null ? null : (int?)src.Profile.ResidenceAddress.UnitNo)
            .Map(dest => dest.ResidenceAddressCertificate, src => src.Profile.ResidenceAddress == null ? null : src.Profile.ResidenceAddress.Certificate)
            .Map(dest => dest.AdditionalAttachments,
                 src => 
                     src.Profile.AdditionalAttachments == null ? null :
                     src.Profile.AdditionalAttachments.Where(a => a.Attachment != null))
            .Map(dest => dest.Qualifications, src => src.Profile.Qualifications == null ? null : src.Profile.Qualifications.OrderBy(q=> q.GraduationYear))
            .Map(dest => dest.Experiences, src => src.Profile.Experiences)
            .Map(dest => dest.TrainingCourses, src => src.Profile.TrainingCourses)
            .Map(dest => dest.Achievements, src => src.Profile.Achievements)
            .Map(dest => dest.Skills, src => src.Profile.Skills)
            .Map(dest => dest.Languages, src => src.Profile.Languages);
    }
}
