namespace Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals.DTOs.Spanshot;

public sealed record PendingExperienceSnapshot
{
    public string? EmployerName { get; init; }
    public string? JobTitle { get; init; }
    public DateOnly? StartDate { get; init; }
    public DateOnly? EndDate { get; init; }
    public Guid? CountryId { get; init; }
    public Guid? CertificateId { get; init; }
    public string? Description { get; init; }
    public Guid? QualificationId { get; init; }
}