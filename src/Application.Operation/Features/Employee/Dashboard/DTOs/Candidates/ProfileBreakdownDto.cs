using Application.Operation.Features.Employee.Dashboard.DTOs.Common;

namespace Application.Operation.Features.Employee.Dashboard.DTOs.Candidates;

public sealed class ProfileBreakdownDto
{
    public IReadOnlyList<StatusCountDto> ByStatus { get; init; } = [];
    public IReadOnlyList<CandidateTypeCountDto> ByCandidateType { get; init; } = [];
    public required CandidateCohortsDto Cohorts { get; init; }
}
