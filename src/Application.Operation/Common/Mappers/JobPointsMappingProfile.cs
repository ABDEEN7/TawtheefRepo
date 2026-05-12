using Application.Operation.Features.Employee.JobManagement.JobOperations.DTOs;
using Application.Operation.Features.Employee.JobManagement.JobCandidates.DTOs;
using Mapster;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Application.Operation.Common.Mappers;

public class JobPointsMappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<JobPointsMainRequestDto, JobPointsMain>()
            .Map(dest => dest.Details, src => src.Details); 

        config.NewConfig<JobPointsDetailCreateDto, JobPointsDetail>();

        config.NewConfig<JobPointsMain, JobPointsMainResponseDto>()
            .Map(dest => dest.Details, src => src.Details)
            .Map(dest => dest.CreatedByName, src => src.CreatedBy != null ? Localize(src.CreatedBy.FullNameAr, src.CreatedBy.FullNameEn) : null);

        config.NewConfig<JobPointsDetail, JobPointsDetailResponseDto>();

        config.NewConfig<JobPointConfigurationResponseDto, JobPointConfiguration>();
        config.NewConfig<JobPointConfigurationRequestDto, JobPointConfiguration>();

        config.NewConfig<JobCategoryCandidateSettings, JobCategoryCandidateSettingsResponseDto>();
        config.NewConfig<JobCategoryCandidateSettingsRequestDto, JobCategoryCandidateSettings>();

        config.NewConfig<InvitationExpiryConfiguration, InvitationExpiryConfigurationResponseDto>();
        config.NewConfig<InvitationExpiryConfigurationRequestDto, InvitationExpiryConfiguration>();
        
        config.NewConfig<JobPointsMain, JobPointsCopyDto>()
            .Map(dest => dest.Details, src => src.Details);

        config.NewConfig<JobPointsDetail, JobPointsDetailCopyDto>();
    }

    private static string Localize(string ar, string en)
    {
        var loc = MapContext.Current!.GetService<ILocalizationService>();
        return loc.GetLocalizedValue(ar, en);
    }
}
