using Cortex.Mediator.Queries;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Operations.Employee.JobCandidates.DTOs;
using Tawtheef.Application.Features.Operations.Employee.JobCandidates.Queries;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Application.Features.Operations.Employee.JobCandidates.Handlers.Queries;

public sealed class GetJobCandidatesOverviewQueryHandler(IUnitOfWork unitOfWork)
    : IQueryHandler<GetJobCandidatesOverviewQuery, IResult<JobCandidatesOverviewDto>>
{
    public async Task<IResult<JobCandidatesOverviewDto>> Handle(
        GetJobCandidatesOverviewQuery request,
        CancellationToken cancellationToken)
    {
        var query = JobCandidatesQueryBuilder.Build(unitOfWork, request.JobId, request.ToFilter());

        var totalCandidates = await query.CountAsync(cancellationToken);
        var availableCandidates = await query.CountAsync(
            candidate => candidate.InvitationStatusId == InvitationStatusIds.Approved,
            cancellationToken);

        var pointsThreshold = request.MinimumPoints ?? 800;
        var abovePointsCandidates = await query.CountAsync(
            candidate => candidate.Points >= pointsThreshold,
            cancellationToken);

        var pointsAverage = totalCandidates == 0
            ? 0
            : await query.AverageAsync(candidate => candidate.Points, cancellationToken);

        var overview = new JobCandidatesOverviewDto
        {
            TotalCandidatesCount = totalCandidates,
            AvailableCandidatesCount = availableCandidates,
            AbovePointsCandidatesCount = abovePointsCandidates,
            PointsAverage = Math.Round(pointsAverage, 2)
        };

        return Result.Ok(overview);
    }
}
