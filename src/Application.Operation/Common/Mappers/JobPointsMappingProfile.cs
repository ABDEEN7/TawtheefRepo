using Application.Operation.Features.Employee.JobManagement.Job.DTOs;
using Mapster;
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
            .Map(dest => dest.Details, src => src.Details); 

        config.NewConfig<JobPointsDetail, JobPointsDetailResponseDto>();

        config.NewConfig<JobPointConfigurationResponseDto, JobPointConfiguration>();
        config.NewConfig<JobPointConfigurationRequestDto, JobPointConfiguration>();
        
        config.NewConfig<JobPointsMain, JobPointsCopyDto>()
            .Map(dest => dest.Details, src => src.Details);

        config.NewConfig<JobPointsDetail, JobPointsDetailCopyDto>();
    }
}
