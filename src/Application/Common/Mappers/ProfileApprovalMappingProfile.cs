using Mapster;
using Tawtheef.Application.Features.Operations.Employee.ProfileApprovals.DTOs;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Common.Mappers;

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
            .Map(dest => dest.Version, src => src.Version)
            .Map(dest => dest.ApprovedAtVersion, src => src.ApprovedAtVersion)
            .Map(dest => dest.ReviewedAtUtc, src => src.ReviewedAtUtc);

        config.NewConfig<UserProfile, BasicInformationSnapshot>()
            .Map(dest => dest.FullNameAr, src => src.User != null ? src.User.FullNameAr : null)
            .Map(dest => dest.FullNameEn, src => src.User != null ? src.User.FullNameEn : null)
            .Map(dest => dest.NationalNumber, src => src.NationalNumber)
            .Map(dest => dest.BirthDate, src => src.BirthDate)
            .Map(dest => dest.Nationality, src => src.Nationality)
            .Map(dest => dest.Gender, src => src.Gender)
            .Map(dest => dest.Religion, src => src.Religion)
            .Map(dest => dest.MaritalStatus, src => src.MaritalStatus)
            .Map(dest => dest.ChildrenCount, src => src.ChildrenCount)
            .Map(dest => dest.CandidateType, src => src.CandidateType)
            .Map(dest => dest.TargetEntity, src => src.TargetEntity)
            .Map(dest => dest.ResumeAttachment, src => src.ResumeAttachment)
            .Map(dest => dest.NationalCard, src => src.NationalCard)
            .Map(dest => dest.ResidenceAddressCertificate, src => src.ResidenceAddressCertificate)
            .Map(dest => dest.BirthdayCertificate, src => src.BirthdayCertificate)
            .Map(dest => dest.MarriageCertificate, src => src.MarriageCertificate);

        config.NewConfig<UserProfile, ProfileApprovalDataDto>()
            .Map(dest => dest.BasicInformation, src => src)
            .Map(dest => dest.ProfilePhoto, src=> src.User!.Avatar)
            .Map(dest => dest.Qualifications, src => src.Qualifications)
            .Map(dest => dest.Experiences, src => src.Experiences)
            .Map(dest => dest.TrainingCourses, src => src.TrainingCourses)
            .Map(dest => dest.ProfessionalCertificatesAndAwards, src => src.Achievements)
            .Map(dest => dest.SkillsAndLanguages, src => src.Skills)
            .Map(dest => dest.Languages, src => src.Languages)
            .Map(dest => dest.Attachments,
                 src => src.AdditionalAttachments == null
                     ? null
                     : src.AdditionalAttachments.Where(a => a.Attachment != null));
    }
}
