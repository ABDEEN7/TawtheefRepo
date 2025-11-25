namespace Tawtheef.Application.Features.Recruitment.Profile.DTOs;

public sealed class SaveProfilePrereqRequest
{
    public Guid CandidateTypeId { get; set; }
    public Guid TargetEntityId { get; set; }

    public Guid? CvResourceId { get; set; }
    public string? CvFileName { get; set; }

    public Guid? IdResourceId { get; set; }
    public string? IdFileName { get; set; }

    public Guid? BirthCertResourceId { get; set; }
    public string? BirthCertFileName { get; set; }

    public Guid? MarriageCertResourceId { get; set; }
    public string? MarriageCertFileName { get; set; }
    public bool Submit { get; set; }
}
