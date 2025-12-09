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
        TypeAdapterConfig<DateTimeOffset?, DateTime?>.NewConfig()
        .MapWith(src => src.HasValue ? src.Value.UtcDateTime : null);
        TypeAdapterConfig<DateTimeOffset, DateTime>.NewConfig()
        .MapWith(src => src.UtcDateTime);

        config.NewConfig<CreateJobDto, Job>()
            .Map(dest => dest.Id, _ => Guid.NewGuid())
            .Map(dest => dest.JobStatusId, _ => JobStatusIds.Draft)
            .Ignore(dest => dest.JobDegrees)
            .Ignore(dest => dest.JobConditions)
            .Ignore(dest => dest.JobSkills)
            .Ignore(dest => dest.JobResponsibilities)
            .Ignore(dest => dest.JobRequiredAttachments)
            .Ignore(dest => dest.JobQuota!)
            .Ignore(dest => dest.OverViewAr!)
            .Ignore(dest => dest.OverViewEn!)
            .Ignore(dest => dest.BenefitsAr!)
            .Ignore(dest => dest.BenefitsEn!)
            .Ignore(dest => dest.QualificationDescriptionAr!)
            .Ignore(dest => dest.QualificationDescriptionEn!);

        TypeAdapterConfig<Job, JobResponseDto>.NewConfig()
            .Map(dest => dest.Status, src => src.JobStatus)
            .Map(dest => dest.Quota, src => src.JobQuota)
            .Map(dest => dest.Degrees, src => src.JobDegrees)
            .Map(dest => dest.Conditions, src => src.JobConditions)
            .Map(dest => dest.Skills, src => src.JobSkills)
            .Map(dest => dest.Responsibilities, src => src.JobResponsibilities)
            .Map(dest => dest.RequiredAttachments, src => src.JobRequiredAttachments);

        TypeAdapterConfig<JobDegree, JobDegreeResponseDto>.NewConfig();
        TypeAdapterConfig<JobCondition, JobConditionResponseDto>.NewConfig();
        TypeAdapterConfig<JobSkill, JobSkillResponseDto>.NewConfig();
        TypeAdapterConfig<JobResponsibility, JobResponsibilityResponseDto>.NewConfig();
        TypeAdapterConfig<JobRequiredAttachment, JobRequiredAttachmentResponseDto>.NewConfig();
        TypeAdapterConfig<JobQuota, JobQuotaResponseDto>.NewConfig();

    }

}
