using Application.Operation.Common.Repositories;
using Application.Operation.Features.Employee.JobManagement.JobCandidates.Commands;
using Application.Operation.Features.Employee.JobManagement.JobCandidates.DTOs;
using Cortex.Mediator.Commands;
using FluentResults;
using MapsterMapper;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Application.Operation.Features.Employee.JobManagement.JobCandidates.Handlers.Commands;

public sealed class SaveJobCategoryCandidateSettingsCommandHandler(
    IJobCategoryCandidateSettingsRepository settingsRepository,
    IUnitOfWork uow,
    IMapper mapper)
    : ICommandHandler<SaveJobCategoryCandidateSettingsCommand, IResult<JobCategoryCandidateSettingsResponseDto>>
{
    public async Task<IResult<JobCategoryCandidateSettingsResponseDto>> Handle(
        SaveJobCategoryCandidateSettingsCommand request,
        CancellationToken cancellationToken)
    {
        var dto = request.Request;
        var existingResult = await settingsRepository.GetAsync();

        JobCategoryCandidateSettings settings;

        if (existingResult.IsSuccess && existingResult.Value is not null)
        {
            settings = existingResult.Value;
            settings.AcademicJobVacancies = dto.AcademicJobVacancies;
            settings.LaborJobVacancies = dto.LaborJobVacancies;
            settings.AdministrativeJobVacancies = dto.AdministrativeJobVacancies;
        }
        else
        {
            settings = new JobCategoryCandidateSettings
            {
                AcademicJobVacancies = dto.AcademicJobVacancies,
                LaborJobVacancies = dto.LaborJobVacancies,
                AdministrativeJobVacancies = dto.AdministrativeJobVacancies
            };

            await settingsRepository.Repository.AddAsync(settings);
        }

        await uow.SaveChangesAsync(cancellationToken);

        var responseDto = mapper.Map<JobCategoryCandidateSettingsResponseDto>(settings);
        return Result.Ok(responseDto);
    }
}
