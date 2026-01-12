namespace Application.Recruitment.Features.JobDetails.DTOs;

public sealed class CandidateJobResponsibilityDto
{
    public Guid Id { get; set; }
    public Guid JobId { get; set; }
    public string? Text { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset? LastModifiedDate { get; set; }
}