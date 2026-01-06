namespace Tawtheef.Application.Features.Operations.Employee.JobCandidates.Models;

public sealed record JobCandidatesFilter(
    string? SearchTerm,
    Guid? GenderId,
    int? MinimumPoints
);
