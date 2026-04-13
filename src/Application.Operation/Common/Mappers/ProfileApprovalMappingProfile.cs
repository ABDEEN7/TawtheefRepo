using Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals.DTOs.ProfileApproval;
using Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals.DTOs.Spanshot;
using Mapster;
using Tawtheef.Domain.Entities.Applicant;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Common.Mappers;

public sealed class ProfileApprovalMappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<ReviewItem, ProfileApprovalItemDto>()
            .Map(dest => dest.ReviewItemId, src => src.Id)
            .Map(dest => dest.TargetType, src => src.TargetType)
            .Map(dest => dest.Status, src => src.Status)
            .Map(dest => dest.Title, src =>
                src.TargetType == ReviewTargetType.Section ? "Textual data"
                    : src.TargetType == ReviewTargetType.Attachment ? (src.AttachmentTitle ?? "Attachment") 
                        : src.TargetType == ReviewTargetType.Row ? (src.EntityName ?? "Row")
                            : src.TargetType == ReviewTargetType.Field  ? (src.FieldPath ?? "Field") 
                                : "Review item")
            .Map(dest => dest.Note, src => src.ReviewerNote)
            .Map(dest => dest.ResourceId, src => src.ResourceId)
            .Map(dest => dest.EntityId, src => src.EntityId)
            .Map(dest => dest.EntityName, src => src.EntityName)
            .Map(dest => dest.OldValue, src => src.ProfileChange != null ? src.ProfileChange.OldValue : null)
            .Map(dest => dest.NewValue, src => src.ProfileChange != null ? src.ProfileChange.NewValue : null)
            .Map(dest => dest.ReviewedAtUtc, src => src.ReviewedAtUtc);

        config.NewConfig<ProfileAdditionalAttachment, AdditionalAttachmentDto>()
            .Map(dest => dest.Title, src => src.FileName)
            .Map(dest => dest.File, src => src.Attachment);

//-----------------

        config.NewConfig<Qualification, QualificationDto>()
            .Map(dest => dest.GradCountryId, src => src.CountryId)
            .Map(dest => dest.GradCountry, src => src.Country)
            .Map(dest => dest.Gpa, src => src.GPA)
            .Map(dest => dest.GradeId, src => src.RatingId)
            .Map(dest => dest.Grade, src => src.Rating)
            .Map(dest => dest.Attachment, src => src.Certificate);
        
        config.NewConfig<Experience, ExperienceDto>()
            .Map(dest => dest.SpecializationRelation, src => src.SpecializationRelation)
            .Map(dest => dest.Attachment, src => src.Certificate)
            .Map(dest => dest.QualificationId, src => src.QualificationId)
            .Map(dest => dest.DegreeName,src=> src.Qualification == null ? null : src.Qualification.Degree)
            .Map(dest => dest.MajorName,src=> src.Qualification == null ? null : src.Qualification.Major)
            .Map(dest => dest.UniversityName,src=> src.Qualification == null ? null : src.Qualification.University)
            .Map(dest => dest.IsCurrent, src => src.EndDate == null);

        config.NewConfig<TrainingCourse, TrainingCourseDto>()
            .Map(dest => dest.SpecializationRelation, src => src.SpecializationRelation)
            .Map(dest => dest.Attachment, src => src.Certificate);

        config.NewConfig<Achievement, AchievementDto>()
            .Map(dest => dest.Attachment, src => src.Attachment)
            .Map(dest => dest.RelatedToSpecialization, src => src.RelatedToSpecialization)
            .Map(dest => dest.AchievementType, src => src.AchievementType);

        config.NewConfig<ProfileSkill, SkillDto>()
            .Map(dest => dest.Skill, src => src.Skill)
            .Map(dest => dest.Level, src => src.Level);

        config.NewConfig<ProfileLanguage, LanguageDto>()
            .Map(dest => dest.Language, src => src.Language)
            .Map(dest => dest.SpeakingLevel, src => src.SpeakingLevel)
            .Map(dest => dest.WritingLevel, src => src.WritingLevel)
            .Map(dest => dest.ReadingLevel, src => src.ReadingLevel);
//-----------------
        config.NewConfig<ResidenceAddress, ResidenceAddressDto>()
            .Map(dest => dest.ResidenceAddressCertificateId, src => src.CertificateId)
            .Map(dest => dest.ResidenceAddressCertificate, src => src.Certificate)
            .Map(dest => dest.naZone, src => src.ZoneNo)
            .Map(dest => dest.naStreet, src => src.StreetNo)
            .Map(dest => dest.naBuilding, src => src.BuildingNo)
            .Map(dest => dest.naUnit, src => src.UnitNo);

        config.NewConfig<UserProfile, BasicInformationSnapshot>()
            .Map(dest => dest.CandidateType, src => src.CandidateType)
            .Map(dest => dest.TargetEntity, src => src.TargetEntity)
            .Map(dest => dest.ResumeAttachment, src => src.ResumeAttachment)
            .Map(dest => dest.NationalCard, src => src.NationalCard)
            .Map(dest => dest.BirthdayCertificate, src => src.BirthdayCertificate)
            .Map(dest => dest.MarriageCertificate, src => src.MarriageCertificate)
            
            .Map(dest => dest.FullNameAr, src => src.User != null ? src.User.FullNameAr : null)
            .Map(dest => dest.FullNameEn, src => src.User != null ? src.User.FullNameEn : null)
            .Map(dest => dest.NationalNumber, src => src.NationalNumber)
            .Map(dest => dest.QidExpiry, src => src.QIDExpiry)
            .Map(dest => dest.BirthDate, src => src.BirthDate)
            .Map(dest => dest.Nationality, src => src.Nationality)
            .Map(dest => dest.Gender, src => src.Gender)
            .Map(dest => dest.Religion, src => src.Religion)
            .Map(dest => dest.MaritalStatus, src => src.MaritalStatus)
            .Map(dest => dest.ChildrenCount, src => src.ChildrenCount)
            .Map(dest => dest.HasDisability, src => src.HasDisability)
            .Map(dest => dest.DisabilityDetails, src => src.DisabilityDetails)
            .Map(dest => dest.SponsorType, src => src.SponsorProfile != null ? src.SponsorProfile.SponsorType : null)
            .Map(dest => dest.SponsorEmployerName, src => src.SponsorProfile != null ? src.SponsorProfile.SponsorName : null)
            .Map(dest => dest.SponsorEmployerNumber, src => src.SponsorProfile != null ? src.SponsorProfile.SponsorNumber : null)
            .Map(dest => dest.SponsorQidExpiry, src => src.SponsorProfile != null ? src.SponsorProfile.QIDExpiry : (DateOnly?)null)
            .Map(dest => dest.SponsorCard, src => src.SponsorProfile != null ? src.SponsorProfile.SponsorCard : null)
            .Map(dest => dest.ResidenceCountry, src => src.ResidenceCountry)
            .Map(dest => dest.InterviewLocation, src => src.InterviewLocation)
            
            
            .Map(dest => dest.PhoneNumber, src => src.User != null ? src.User.PhoneNumber : null)
            .Map(dest => dest.Email, src => src.User != null ? src.User.Email : null)
            .Map(dest => dest.Address, src => src.Address)
            .Map(dest => dest.ResidenceAddress, src => src.ResidenceAddress);

        config.NewConfig<UserProfile, ProfileApprovalDataDto>()
            .Map(dest => dest.BasicInformation, src => src)
            .Map(dest => dest.ProfilePhoto, src=> src.User!.Avatar)
            .Map(dest => dest.Qualifications, src => src.Qualifications!.OrderByDescending(q => q.GraduationYear))
            .Map(dest => dest.Experiences, src => src.Experiences!.OrderByDescending(q => q.StartDate))
            .Map(dest => dest.TrainingCourses, src => src.TrainingCourses!.OrderByDescending(q => q.StartDate))
            .Map(dest => dest.ProfessionalCertificatesAndAwards, src => src.Achievements!.OrderByDescending(q => q.IssueDate))
            .Map(dest => dest.Skills, src => src.Skills)
            .Map(dest => dest.Languages, src => src.Languages)
            .Map(dest => dest.Attachments,
                 src => src.AdditionalAttachments!.Where(a => a.Attachment != null));
    }
}
