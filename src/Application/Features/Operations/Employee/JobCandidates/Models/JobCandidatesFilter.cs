namespace Tawtheef.Application.Features.Operations.Employee.JobCandidates.Models;

public sealed record JobCandidatesFilter(
    string? SearchTerm,
    Guid? JobCategoryId,
    Guid? CandidateTypeId,
    int? MinimumPoints
);
