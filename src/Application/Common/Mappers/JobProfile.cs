using Mapster;
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
            .Ignore(dest => dest.JobResponsibilities)
            .Ignore(dest => dest.JobRequiredAttachments)
            .Ignore(dest => dest.OverViewAr!)
            .Ignore(dest => dest.OverViewEn!)
            .Ignore(dest => dest.BenefitsAr!)
            .Ignore(dest => dest.BenefitsEn!)
            .Ignore(dest => dest.QualificationDescriptionAr!)
            .Ignore(dest => dest.QualificationDescriptionEn!);

        config.NewConfig<Job, JobResponseDto>()
            .Map(dest => dest.JobStatus, src => src.JobStatus)
            .Map(dest => dest.Degrees, src => src.JobDegrees)
            .Map(dest => dest.Conditions, src => src.JobConditions)
            .Map(dest => dest.Skills, src => src.JobSkills)
            .Map(dest => dest.Responsibilities, src => src.JobResponsibilities)
            .Map(dest => dest.RequiredAttachments, src => src.JobRequiredAttachments)
            .Map(dest => dest.TabReviewNotes, src => src.TabReviewNotes);

        config.NewConfig<JobDegree, JobDegreeResponseDto>();
        config.NewConfig<JobCondition, JobConditionResponseDto>();
        config.NewConfig<JobSkill, JobSkillResponseDto>();
        config.NewConfig<JobResponsibility, JobResponsibilityResponseDto>();
        config.NewConfig<JobRequiredAttachment, JobRequiredAttachmentResponseDto>();
        config.NewConfig<JobTabReviewNote, JobTabReviewNoteResponseDto>()
            .Map(dest => dest.Attachments, src => src.Attachments.Where(a=>a.Attachment != null).Select(a=>a.Attachment!));
    }

}
