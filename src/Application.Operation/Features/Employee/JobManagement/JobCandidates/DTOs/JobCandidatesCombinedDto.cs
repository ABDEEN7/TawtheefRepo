using Tawtheef.Application.Common.Models.Pagination;

namespace Application.Operation.Features.Employee.JobCandidates.DTOs;

public sealed class JobCandidatesCombinedDto
{
    public required PaginatedResult<JobCandidateListItemDto> List { get; init; }
    public required JobCandidatesOverviewDto Overview { get; init; }
}
