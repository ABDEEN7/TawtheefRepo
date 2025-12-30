using FluentResults;
using MapsterMapper;
using MediatR;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Operations.Employee.Job.Commands;
using Tawtheef.Application.Features.Operations.Employee.Job.DTOs;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Handlers.Commands;

public class AddJobPointsConfigurationCommandHandler(
    IJobPointsConfigurationsRepository jobPointsRepository,
    IUnitOfWork uow,
    IMapper mapper
) : IRequestHandler<AddJobPointsConfigurationCommand, IResult<JobPointConfigurationResponseDto>>
{
    public async Task<IResult<JobPointConfigurationResponseDto>> Handle(AddJobPointsConfigurationCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Request;

        var existingResult = await jobPointsRepository.GetByJobIdAsync(dto.JobId);
        if (existingResult.IsSuccess)
            return Result.Fail<JobPointConfigurationResponseDto>(JobMessages.JobPointsAlreadyExist);

        var jobPointsConfig = new JobPointConfiguration
        {
            JobId = dto.JobId,
            ApplicantCategoryMaxPoints = dto.ApplicantCategoryMaxPoints,
            EducationMaxPoints = dto.EducationMaxPoints,
            ExperienceMaxPoints = dto.ExperienceMaxPoints,
            TrainingMaxPoints = dto.TrainingMaxPoints,
            CertificatesMaxPoints = dto.CertificatesMaxPoints,
            SkillsMaxPoints = dto.SkillsMaxPoints,
            LanguagesMaxPoints = dto.LanguagesMaxPoints,
            MaxPoints =dto.MaxPoints,
            
        };

        if (!jobPointsConfig.IsValid())
            return Result.Fail<JobPointConfigurationResponseDto>(JobMessages.JobPointsTotalNotValid);

        await jobPointsRepository.Repository.AddAsync(jobPointsConfig);
        await uow.SaveChangesAsync(cancellationToken);

        var responseDto = mapper.Map<JobPointConfigurationResponseDto>(jobPointsConfig);

        return Result.Ok(responseDto);
    }
}
