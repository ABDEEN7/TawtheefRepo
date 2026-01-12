namespace Application.Operation.Features.Employee.JobCandidates.Models;

public sealed record JobCandidatesFilter(
    string? SearchTerm,
    Guid? GenderId,
    int? MinimumPoints
);
