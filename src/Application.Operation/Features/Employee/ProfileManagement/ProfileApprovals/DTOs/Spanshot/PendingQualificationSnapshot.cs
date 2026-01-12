namespace Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals.DTOs.Spanshot;

public sealed record PendingQualificationSnapshot
{
    public Guid? DegreeId { get; init; }
    public Guid? GradCountryId { get; init; }
    public Guid? UniversityId { get; init; }
    public Guid? MajorId { get; init; }
    public Guid? SubMajorId { get; init; }
    public Guid? StudyTypeId { get; init; }
    public Guid? GradeId { get; init; }
    public int? GradYear { get; init; }
    public decimal? Gpa { get; init; }
    public Guid? AttachmentResourceId { get; init; }
}