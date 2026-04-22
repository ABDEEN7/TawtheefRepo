using Application.Recruitment.Features.JobDetails.DTOs;
using Mapster;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Application.Recruitment.Common.Mappers;

public class JobProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<JobDegree, JobDegreeResponseDto>();
        config.NewConfig<JobSkill, JobSkillResponseDto>();
        config.NewConfig<JobSpecialization, JobSpecializationResponseDto>();
        config.NewConfig<JobReviewAttachment, FileRefDto>()
            .Map(dest => dest.ResourceId, src => src.AttachmentId)
            .Map(dest => dest.FileName, src => src.FileName)
            .Map(dest => dest.Url, src => src.Attachment != null ? src.Attachment.Url : string.Empty);
        
        config.NewConfig<Job, CandidateJobDetailsDto>()
            .Map(d => d.Title, s => Localize(s.JobTitle!.JobNameAr, s.JobTitle!.JobNameEn))
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
            .Map(d => d.RequiredAttachments, s => s.JobRequiredAttachments)
            .Map(d => d.JobSpecializations, s => s.JobSpecializations);

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
