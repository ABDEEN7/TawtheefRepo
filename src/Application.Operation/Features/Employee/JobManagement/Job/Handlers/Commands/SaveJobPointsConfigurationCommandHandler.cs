using Application.Operation.Common.Repositories;
using Application.Operation.Features.Employee.Job.Commands;
using Application.Operation.Features.Employee.Job.DTOs;
using Cortex.Mediator.Commands;
using FluentResults;
using MapsterMapper;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Application.Operation.Features.Employee.Job.Handlers.Commands;

public class SaveJobPointsConfigurationCommandHandler(
    IJobPointsConfigurationsRepository jobPointsRepository,
    IUnitOfWork uow,
    IMapper mapper
) : ICommandHandler<SaveJobPointsConfigurationCommand, IResult<JobPointConfigurationResponseDto>>
{
   public async Task<IResult<JobPointConfigurationResponseDto>> Handle(
       SaveJobPointsConfigurationCommand request,
    CancellationToken cancellationToken)
{
    var dto = request.Request;

    var existingResult = await jobPointsRepository.GetAsync();

    JobPointConfiguration jobPointsConfig;

    if (existingResult.IsSuccess && existingResult.Value is not null)
    {
        jobPointsConfig = existingResult.Value;

        jobPointsConfig.ApplicantCategoryMaxPoints = dto.ApplicantCategoryMaxPoints;
        jobPointsConfig.EducationMaxPoints = dto.EducationMaxPoints;
        jobPointsConfig.ExperienceMaxPoints = dto.ExperienceMaxPoints;
        jobPointsConfig.TrainingMaxPoints = dto.TrainingMaxPoints;
        jobPointsConfig.CertificatesMaxPoints = dto.CertificatesMaxPoints;
        jobPointsConfig.SkillsMaxPoints = dto.SkillsMaxPoints;
        jobPointsConfig.LanguagesMaxPoints = dto.LanguagesMaxPoints;
        jobPointsConfig.MaxPoints = dto.MaxPoints;

        if (!jobPointsConfig.IsValid())
            return Result.Fail<JobPointConfigurationResponseDto>(JobMessages.JobPointsTotalNotValid);
    }
    else
    {
        jobPointsConfig = new JobPointConfiguration
        {
            ApplicantCategoryMaxPoints = dto.ApplicantCategoryMaxPoints,
            EducationMaxPoints = dto.EducationMaxPoints,
            ExperienceMaxPoints = dto.ExperienceMaxPoints,
            TrainingMaxPoints = dto.TrainingMaxPoints,
            CertificatesMaxPoints = dto.CertificatesMaxPoints,
            SkillsMaxPoints = dto.SkillsMaxPoints,
            LanguagesMaxPoints = dto.LanguagesMaxPoints,
            MaxPoints = dto.MaxPoints,
        };

        if (!jobPointsConfig.IsValid())
            return Result.Fail<JobPointConfigurationResponseDto>(JobMessages.JobPointsTotalNotValid);

        await jobPointsRepository.Repository.AddAsync(jobPointsConfig);
    }

    await uow.SaveChangesAsync(cancellationToken);

    var responseDto = mapper.Map<JobPointConfigurationResponseDto>(jobPointsConfig);
    return Result.Ok(responseDto);
}
}
