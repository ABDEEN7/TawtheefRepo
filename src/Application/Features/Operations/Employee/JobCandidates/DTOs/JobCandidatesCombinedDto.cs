using Tawtheef.Application.Common.Models.Pagination;

namespace Tawtheef.Application.Features.Operations.Employee.JobCandidates.DTOs;

public sealed class JobCandidatesCombinedDto
{
    public required PaginatedResult<JobCandidateListItemDto> List { get; init; }
    public required JobCandidatesOverviewDto Overview { get; init; }
}
