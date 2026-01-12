namespace Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals.DTOs.Spanshot;

public sealed record PendingAchievementSnapshot
{
    public Guid AchievementTypeId { get; init; }
    public string? Title { get; init; }
    public string? IssuingAuthority { get; init; }
    public Guid? CountryId { get; init; }
    public DateOnly? IssueDate { get; init; }
    public string? Description { get; init; }
    public bool? RelatedToSpecialization { get; init; }
    public Guid? AttachmentResourceId { get; init; }
}
