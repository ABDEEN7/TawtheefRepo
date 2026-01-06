using Mapster;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;
using Tawtheef.Application.Features.Operations.Employee.Job.DTOs;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Tawtheef.Application.Common.Mappers;

public class JobProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<DateTimeOffset?, DateTime?>()
        .MapWith(src => src.HasValue ? src.Value.UtcDateTime : null);
        config.NewConfig<DateTimeOffset, DateTime>()
        .MapWith(src => src.UtcDateTime);

        config.NewConfig<CreateJobDto, Job>()
            .Map(dest => dest.Id, _ => Guid.NewGuid())
            .Map(dest => dest.JobStatusId, _ => JobStatusIds.Draft)
            .Ignore(dest => dest.JobDegrees)
            .Ignore(dest => dest.JobConditions)
            .Ignore(dest => dest.JobSkills)
            .Ignore(dest => dest.JobPoints!)
            .Ignore(dest => dest.JobResponsibilities)
            .Ignore(dest => dest.JobRequiredAttachments)
            .Ignore(dest => dest.OverViewAr!)
            .Ignore(dest => dest.OverViewEn!)
            .Ignore(dest => dest.BenefitsAr!)
            .Ignore(dest => dest.BenefitsEn!)
            .Ignore(dest => dest.QualificationDescriptionAr!)
            .Ignore(dest => dest.QualificationDescriptionEn!);
        
        config.NewConfig<CreateJobFromPreviousDto, Job>()
            .Map(dest => dest.Id, _ => Guid.NewGuid())
            .Map(dest => dest.JobStatusId, _ => JobStatusIds.PendingApproval)
            .Map(dest => dest.OverViewAr, src => src.OverviewAr)
            .Map(dest => dest.OverViewEn, src => src.OverviewEn)
            .Map(dest => dest.QualificationDescriptionAr, src => src.QualificationsDescriptionAr)
            .Map(dest => dest.QualificationDescriptionEn, src => src.QualificationsDescriptionEn)
            .Ignore(dest => dest.JobDegrees)
            .Ignore(dest => dest.JobConditions)
            .Ignore(dest => dest.JobSkills)
            .Ignore(dest => dest.JobPoints!)
            .Ignore(dest => dest.JobResponsibilities)
            .Ignore(dest => dest.JobRequiredAttachments)
            .Ignore(dest => dest.Invitations)
            .Ignore(dest => dest.TabReviewNotes)
            .Ignore(dest => dest.ReviewAttachment!);

        config.NewConfig<Job, JobResponseDto>()
            .Map(dest => dest.JobStatus, src => src.JobStatus)
            .Map(dest => dest.Degrees, src => src.JobDegrees)
            .Map(dest => dest.Conditions, src => src.JobConditions)
            .Map(dest => dest.Skills, src => src.JobSkills)
            .Map(dest => dest.Responsibilities, src => src.JobResponsibilities)
            .Map(dest => dest.RequiredAttachments, src => src.JobRequiredAttachments)
            .Map(dest => dest.TabReviewNotes, src => src.TabReviewNotes)
            .Map(dest => dest.ReviewAttachments, src => src.ReviewAttachment);
        
        config.NewConfig<Job, JobCopyTemplateDto>()
            .Map(dest => dest.OverviewAr, src => src.OverViewAr)
            .Map(dest => dest.OverviewEn, src => src.OverViewEn)
            .Map(dest => dest.QualificationsDescriptionAr, src => src.QualificationDescriptionAr)
            .Map(dest => dest.QualificationsDescriptionEn, src => src.QualificationDescriptionEn)
            .Map(dest => dest.Degrees, src => src.JobDegrees)
            .Map(dest => dest.Conditions, src => src.JobConditions)
            .Map(dest => dest.Skills, src => src.JobSkills)
            .Map(dest => dest.Responsibilities, src => src.JobResponsibilities)
            .Map(dest => dest.RequiredAttachments, src => src.JobRequiredAttachments);

        config.NewConfig<JobDegree, JobDegreeResponseDto>();
        config.NewConfig<JobCondition, JobConditionResponseDto>();
        config.NewConfig<JobSkill, JobSkillResponseDto>();
        config.NewConfig<JobResponsibility, JobResponsibilityResponseDto>();
        config.NewConfig<JobRequiredAttachment, JobRequiredAttachmentResponseDto>();
        config.NewConfig<JobTabReviewNote, JobTabReviewNoteResponseDto>();
        config.NewConfig<JobTabReviewNote, JobTabReviewUpsertDto>()
            .Map(dest => dest.Status, src => src.TabStatus);
        config.NewConfig<JobTabReviewUpsertDto, JobTabReviewNote>()
            .Map(dest => dest.TabStatus, src => src.Status);
        config.NewConfig<JobReviewAttachment, FileRefDto>()
            .Map(dest => dest.ResourceId, src => src.AttachmentId)
            .Map(dest => dest.FileName, src => src.FileName)
            .Map(dest => dest.Url, src => src.Attachment != null ? src.Attachment.Url : string.Empty);
        
        config.NewConfig<JobDegree, JobDegreeRequestDto>()
            .Map(dest => dest.DegreeId, src => src.DegreeId);
        config.NewConfig<JobCondition, JobConditionRequestDto>();
        config.NewConfig<JobSkill, JobSkillRequestDto>();
        config.NewConfig<JobResponsibility, JobResponsibilityRequestDto>();
        config.NewConfig<JobRequiredAttachment, JobRequiredAttachmentRequestDto>();
    }

}
