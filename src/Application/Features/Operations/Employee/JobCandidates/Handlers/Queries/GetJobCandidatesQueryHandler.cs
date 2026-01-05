using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Features.Operations.Employee.JobCandidates.DTOs;
using Tawtheef.Application.Features.Operations.Employee.JobCandidates.Models;
using Tawtheef.Application.Features.Operations.Employee.JobCandidates.Queries;
using Tawtheef.Application.Features.Operations.Employee.JobCandidates.Services;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Tawtheef.Application.Features.Operations.Employee.JobCandidates.Handlers.Queries;

public sealed class GetJobCandidatesQueryHandler(
    IUnitOfWork unitOfWork,
    ILocalizationService localizationService,
    IJobPointsRepository jobPointsRepository)
    : IRequestHandler<GetJobCandidatesQuery, IResult<PaginatedResult<JobCandidateListItemDto>>>
{
    private readonly JobCandidatePointsCalculator _pointsCalculator = new();

    public async Task<IResult<PaginatedResult<JobCandidateListItemDto>>> Handle(
        GetJobCandidatesQuery request,
        CancellationToken cancellationToken)
    {
        var query = JobCandidatesQueryBuilder.Build(unitOfWork, request.JobId, request.Filter);

        var candidates = await query.ToListAsync(cancellationToken);
        var jobPoints = await LoadJobPointsAsync(request.JobId);

        var job = await unitOfWork.GetEntityRepository<Domain.Entities.Recruitment.Job>().DbSet
            .AsNoTracking()
            .Include(j => j.Department)
            .Include((j => j.JobSkills))
            .ThenInclude(s=>s.Skill)
            .Include(j => j.JobCategory)
            .FirstOrDefaultAsync(j => j.Id == request.JobId, cancellationToken);

        var candidatesWithPoints = candidates
            .Select(candidate => candidate with { Points = _pointsCalculator.Calculate(candidate, jobPoints) })
            .ToList();

        if (request.Filter?.MinimumPoints is { } minPoints)
        {
            candidatesWithPoints = candidatesWithPoints
                .Where(candidate => candidate.Points >= minPoints)
                .ToList();
        }

        var paginated = CreatePaginatedResult(candidatesWithPoints, request.Pagination);

        var items = paginated.Items.Select(candidate => new JobCandidateListItemDto
        {
            InvitationId = candidate.InvitationId,
            CandidateId = candidate.ApplicantId,
            CandidateName = localizationService.GetLocalizedFullName(candidate.Applicant),
            Department = localizationService.GetLocalizedName(job?.Department),
            JobCategory = localizationService.GetLocalizedName(job?.JobCategory),
            CandidateCategory = localizationService.GetLocalizedName(candidate.Profile?.CandidateType),
            CandidateMajor = localizationService.GetLocalizedName(candidate.Major),
            CandidateGender = localizationService.GetLocalizedName(candidate.Profile?.Gender),
            Points = candidate.Points
        }).ToList();

        var result = new PaginatedResult<JobCandidateListItemDto>(
            items,
            paginated.Metadata.TotalCount,
            paginated.Metadata.CurrentPage,
            paginated.Metadata.PageSize);

        return Result.Ok(result);
    }

    private async Task<JobPointsMain?> LoadJobPointsAsync(Guid jobId)
    {
        var result = await jobPointsRepository.GetByJobIdAsync(jobId);
        return result.IsSuccess ? result.Value : null;
    }

    private static PaginatedResult<JobCandidateRecord> CreatePaginatedResult(
        IReadOnlyList<JobCandidateRecord> candidates,
        PaginatedRequest pagination)
    {
        var sorted = SortCandidates(candidates, pagination.SortBy, pagination.SortDirection);
        var totalCount = sorted.Count;

        var items = sorted
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToList();

        return new PaginatedResult<JobCandidateRecord>(
            items,
            totalCount,
            pagination.PageNumber,
            pagination.PageSize);
    }

    private static List<JobCandidateRecord> SortCandidates(
        IReadOnlyList<JobCandidateRecord> candidates,
        string? sortBy,
        string? sortDirection)
    {
        if (string.IsNullOrWhiteSpace(sortBy))
        {
            return candidates.ToList();
        }

        var isDescending = string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase);

        if (string.Equals(sortBy, nameof(JobCandidateRecord.CreatedDate), StringComparison.OrdinalIgnoreCase) ||
            string.Equals(sortBy, "createdDate", StringComparison.OrdinalIgnoreCase))
        {
            // CreatedDate is nullable now: handle nulls
            return isDescending
                ? candidates.OrderByDescending(c => c.CreatedDate ?? DateTime.MinValue).ToList()
                : candidates.OrderBy(c => c.CreatedDate ?? DateTime.MinValue).ToList();
        }

        return candidates.ToList();
    }
}
