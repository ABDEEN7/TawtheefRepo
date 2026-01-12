using Application.Operation.Features.Employee.Job.DTOs;
using Mapster;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Application.Operation.Common.Mappers;

public class JobProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
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
        
        config.NewConfig<Job, CandidateJobDetailsDto>()
            .Map(d => d.Title, s => Localize(s.TitleAr, s.TitleEn))
            .Map(d => d.Benefits, s => Localize(s.BenefitsAr ?? string.Empty, s.BenefitsEn ?? string.Empty))
            .Map(d => d.OverView, s => Localize(s.OverViewAr ?? string.Empty, s.OverViewEn ?? string.Empty))
            .Map(d => d.QualificationDescription, s =>
                Localize(s.QualificationDescriptionAr ?? string.Empty, s.QualificationDescriptionEn ?? string.Empty))

            // DropdownOptions via existing mappings (assumes you already map entity -> DropdownOptions)
            .Map(d => d.Sector, s => s.Sector)
            .Map(d => d.Management, s => s.Management)
            .Map(d => d.Department, s => s.Department)
            .Map(d => d.JobCategory, s => s.JobCategory)
            .Map(d => d.Gender, s => s.Gender)
            .Map(d => d.WorkLocation, s => s.WorkLocation)
            .Map(d => d.Major, s => s.Major)
            .Map(d => d.SubMajor, s => s.SubMajor)
            .Map(d => d.WorkType, s => s.WorkType)
            .Map(d => d.JobStatus, s => s.JobStatus)

            // JobPoints and collections
            .Map(d => d.JobPoints, s => s.JobPoints)
            .Map(d => d.Degrees, s => s.JobDegrees)
            .Map(d => d.Skills, s => s.JobSkills)

            // Localized nested collections (manual per-item mapping but still inside Mapster)
            .Map(d => d.Conditions, s => s.JobConditions)
            .Map(d => d.Responsibilities, s => s.JobResponsibilities)
            .Map(d => d.RequiredAttachments, s => s.JobRequiredAttachments);

        config.NewConfig<JobCondition, CandidateJobConditionDto>()
            .Map(d => d.Text, s => Localize(s.TextAr, s.TextEn));

        config.NewConfig<JobResponsibility, CandidateJobResponsibilityDto>()
            .Map(d => d.Text, s => Localize(s.TextAr, s.TextEn));

        config.NewConfig<JobRequiredAttachment, CandidateJobRequiredAttachmentDto>()
            .Map(d => d.Title, s => Localize(s.TitleAr, s.TitleEn));
    }
    
    private static string Localize(string ar, string en)
    {
        var loc = MapContext.Current!.GetService<ILocalizationService>();
        return loc.GetLocalizedValue(ar, en);
    }

}
