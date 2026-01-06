using Cortex.Mediator.Queries;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Operations.Employee.JobCandidates.DTOs;
using Tawtheef.Application.Features.Operations.Employee.JobCandidates.Queries;
using Tawtheef.Application.Features.Operations.Employee.JobCandidates.Services;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Tawtheef.Application.Features.Operations.Employee.JobCandidates.Handlers.Queries;

public sealed class GetJobCandidatesOverviewQueryHandler(IUnitOfWork unitOfWork,IJobPointsRepository jobPointsRepository)
    : IRequestHandler<GetJobCandidatesOverviewQuery, IResult<JobCandidatesOverviewDto>>
{
    private readonly JobCandidatePointsCalculator _pointsCalculator = new();

    public async Task<IResult<JobCandidatesOverviewDto>> Handle(
        GetJobCandidatesOverviewQuery request,
        CancellationToken cancellationToken)
    {
        var query = JobCandidatesQueryBuilder.Build(unitOfWork, request.JobId, request.ToFilter());

        var candidates = await query.ToListAsync(cancellationToken);
        var jobPoints = await LoadJobPointsAsync(request.JobId, cancellationToken);

        var candidatesWithPoints = candidates
            .Select(candidate => candidate with { Points = _pointsCalculator.Calculate(candidate, jobPoints) })
            .ToList();

        if (request.MinimumPoints is { } minPoints)
        {
            candidatesWithPoints = candidatesWithPoints
                .Where(candidate => candidate.Points >= minPoints)
                .ToList();
        }

        var totalCandidates = candidatesWithPoints.Count;
        var availableCandidates = candidatesWithPoints.Count(
            candidate => candidate.InvitationStatusId == InvitationStatusIds.Approved);

        var pointsThreshold = request.MinimumPoints ?? 800;
        var abovePointsCandidates = candidatesWithPoints.Count(
            candidate => candidate.Points >= pointsThreshold);

        var pointsAverage = totalCandidates == 0
                ? 0
            : candidatesWithPoints.Average(candidate => candidate.Points);

        var overview = new JobCandidatesOverviewDto
        {
            TotalCandidatesCount = totalCandidates,
            AvailableCandidatesCount = availableCandidates,
            AbovePointsCandidatesCount = abovePointsCandidates,
            PointsAverage = Math.Round(pointsAverage, 2)
        };

        return Result.Ok(overview);
    }
    
    private async Task<JobPointsMain?> LoadJobPointsAsync(Guid jobId, CancellationToken cancellationToken)
    {
        var result = await jobPointsRepository.GetByJobIdAsync(jobId);
        return result.IsSuccess ? result.Value : null;
    }
}
