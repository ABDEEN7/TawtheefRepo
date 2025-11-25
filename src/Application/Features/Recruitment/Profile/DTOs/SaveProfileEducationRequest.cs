namespace Tawtheef.Application.Features.Recruitment.Profile.DTOs;

public sealed class SaveProfileEducationRequest
{
    public bool Submit { get; set; }

    public List<QualificationUpsertDto> Degrees { get; set; } = [];
}

public sealed class QualificationUpsertDto
{
    public Guid? Id { get; set; }
    public Guid LevelId { get; set; }
    public Guid MajorId { get; set; }
    public Guid UniversityId { get; set; }
    public int? GraduationYear { get; set; }
    public Guid StudyTypeId { get; set; }
    public string GPA { get; set; } = default!;
    public Guid RatingId { get; set; }
    public Guid CountryId { get; set; }
    
    public Guid? CertificateResourceId { get; set; }
}
