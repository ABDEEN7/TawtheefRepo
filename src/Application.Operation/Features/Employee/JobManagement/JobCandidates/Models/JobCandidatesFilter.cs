namespace Application.Operation.Features.Employee.JobManagement.JobCandidates.Models;

public sealed record JobCandidatesFilter(
    string? SearchTerm,
    Guid? GenderId,
    int? MinimumPoints,
    IReadOnlyCollection<JobSpecialization>? SelectedSpecializations = null
);