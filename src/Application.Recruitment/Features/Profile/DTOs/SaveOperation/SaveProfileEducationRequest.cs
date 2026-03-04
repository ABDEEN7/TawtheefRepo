using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Application.Recruitment.Features.Profile.DTOs.SaveOperation;

public sealed record SaveProfileEducationRequest
{
    public bool Submit { get; set; }

    [RegularExpression(@"^[^\<\>]*$", ErrorMessage = "Invalid characters in degrees.")]
    public required string DegreesJson { get; set; }
    public List<IFormFile?> DegreeFiles { get; set; } = [];
}

public sealed record SaveProfileEducationDegreeDto
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

    [RegularExpression(@"^[a-zA-Z0-9\s\,\.\-\(\)]+$", ErrorMessage = "File name contains invalid characters.")]
    public string? ExistingFileName { get; set; }
}

