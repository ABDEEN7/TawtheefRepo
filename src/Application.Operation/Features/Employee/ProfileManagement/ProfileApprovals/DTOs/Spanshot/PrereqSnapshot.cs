namespace Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals.DTOs.Spanshot;

public sealed record PrereqSnapshot
{
    public Guid? CandidateTypeId { get; init; }
    public Guid? TargetEntityId { get; init; }
    public Guid? OfficeId { get; init; }
    public DateOnly? QidExpiry { get; init; }
    public Guid? ResumeAttachmentId { get; init; }
    public Guid? NationalCardId { get; init; }
    public Guid? BirthCertificateId { get; init; }
    public Guid? MarriageCertificateId { get; init; }
}