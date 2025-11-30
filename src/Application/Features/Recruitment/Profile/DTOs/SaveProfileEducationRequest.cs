using Microsoft.AspNetCore.Http;

namespace Tawtheef.Application.Features.Recruitment.Profile.DTOs;

public sealed class SaveProfileEducationRequest
{
    public bool Submit { get; set; }
    public required string DegreesJson { get; set; }
    public required List<IFormFile> DegreeFiles { get; set; }
}

public sealed class SaveProfileEducationDegreeDto
{
    public Guid DegreeId { get; set; }
    public Guid GradCountryId { get; set; }
    public Guid UniversityId { get; set; }
    public Guid MajorId { get; set; }
    public Guid SubMajorId { get; set; }
    public Guid StudyTypeId { get; set; }
    public Guid GradeId { get; set; }
    public int GradYear { get; set; }
    public decimal Gpa { get; set; }

    public string FileName { get; set; } = string.Empty;
}
