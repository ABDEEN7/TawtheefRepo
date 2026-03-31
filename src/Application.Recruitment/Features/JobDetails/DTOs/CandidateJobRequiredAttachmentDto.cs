namespace Application.Recruitment.Features.JobDetails.DTOs;

public sealed class CandidateJobRequiredAttachmentDto
{
    public Guid Id { get; set; }
    public Guid JobId { get; set; }
    public string? Title { get; set; }
    public bool IsMandatory { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset? LastModifiedDate { get; set; }
    
    // Candidate's uploaded attachment info
    public Guid? AttachmentId { get; set; }
    public Guid? ResourceId { get; set; }
    public string? ResourceUrl { get; set; }
    public string? ResourceName { get; set; }
    public bool? IsApproved { get; set; }
    public bool? IsReturned { get; set; }
    public string? ReviewNote { get; set; }
}