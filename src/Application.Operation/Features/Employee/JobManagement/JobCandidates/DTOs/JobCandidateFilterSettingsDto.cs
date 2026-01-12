namespace Application.Operation.Features.Employee.JobCandidates.DTOs;

public sealed class JobCandidateFilterSettingsDto
{
    public Guid JobId { get; init; }
    public Guid? GenderId { get; init; }
    public int? MinimumPoints { get; init; }
    public List<JobCandidateTypePercentageDto> CandidateTypePercentages { get; init; } = [];
    public List<JobCandidateNationalityPercentageDto> NationalityPercentages { get; init; } = [];
}

public sealed class JobCandidateTypePercentageDto
{
    public Guid CandidateTypeId { get; init; }
    public int Percentage { get; init; }
}

public sealed class JobCandidateNationalityPercentageDto
{
    public Guid CandidateTypeId { get; init; }
    public Guid NationalityId { get; init; }
    public int Percentage { get; init; }
}
