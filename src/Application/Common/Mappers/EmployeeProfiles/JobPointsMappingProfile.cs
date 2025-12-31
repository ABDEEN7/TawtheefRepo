using Mapster;
using Tawtheef.Application.Features.Operations.Employee.Job.DTOs;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Tawtheef.Application.Common.Mappers.EmployeeProfiles;

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
    }
}
