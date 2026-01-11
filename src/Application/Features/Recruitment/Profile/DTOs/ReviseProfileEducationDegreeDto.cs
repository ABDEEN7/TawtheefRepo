namespace Tawtheef.Application.Features.Recruitment.Profile.DTOs;

public sealed record ReviseProfileEducationDegreeDto
{
    public Guid? Id { get; set; }
    public Guid DegreeId { get; set; }
    public Guid GradCountryId { get; set; }
    public Guid? UniversityId { get; set; }
    public Guid? MajorId { get; set; }
    public Guid? SubMajorId { get; set; }
    public Guid? StudyTypeId { get; set; }
    public Guid? GradeId { get; set; }
    public int? GradYear { get; set; }
    public decimal? Gpa { get; set; }
    public Guid? CertificateId { get; set; }
    public int? FileIndex { get; set; }
    public string? ExistingFileName { get; set; }
}
