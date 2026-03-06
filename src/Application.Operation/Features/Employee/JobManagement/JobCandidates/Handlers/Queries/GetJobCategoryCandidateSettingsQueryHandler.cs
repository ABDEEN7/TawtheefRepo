using Application.Operation.Common.Repositories;
using Application.Operation.Features.Employee.JobManagement.JobCandidates.DTOs;
using Application.Operation.Features.Employee.JobManagement.JobCandidates.Queries;
using MediatR;
using FluentResults;
using MapsterMapper;

namespace Application.Operation.Features.Employee.JobManagement.JobCandidates.Handlers.Queries;

public sealed class GetJobCategoryCandidateSettingsQueryHandler(
    IJobCategoryCandidateSettingsRepository settingsRepository,
    IMapper mapper)
    : IRequestHandler<GetJobCategoryCandidateSettingsQuery, IResult<JobCategoryCandidateSettingsResponseDto>>
{
    public async Task<IResult<JobCategoryCandidateSettingsResponseDto>> Handle(
        GetJobCategoryCandidateSettingsQuery request,
        CancellationToken cancellationToken)
    {
        var result = await settingsRepository.GetAsync();

        if (result.IsFailed)
            return Result.Fail<JobCategoryCandidateSettingsResponseDto>(result.Errors);

        var settings = result.Value;
        var dto = mapper.Map<JobCategoryCandidateSettingsResponseDto>(settings);

        return Result.Ok(dto);
    }
}

