using Tawtheef.Application.Features.Authenticator.DTOs.Responses;
using ResidenceAddressDto = Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals.DTOs.ProfileApproval.ResidenceAddressDto;

namespace Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals.DTOs.Spanshot;

public record BasicInformationSnapshot
{
    public string? CandidateType { get; init; }
    public string? TargetEntity { get; init; }
    public string? Office { get; init; }
    public FileRefDto ResumeAttachment { get; set; } = new();
    public FileRefDto? NationalCard { get; set; }
    public FileRefDto? BirthdayCertificate { get; set; }
    public FileRefDto? MarriageCertificate { get; set; }

    public string? FullNameAr { get; init; }
    public string? FullNameEn { get; init; }
    public string? NationalNumber { get; init; }
    public DateOnly? QidExpiry { get; init; }
    public DateOnly? BirthDate { get; init; }
    public string? Nationality { get; init; }
    public string? Gender { get; init; }
    public string? Religion { get; init; }
    public string? MaritalStatus { get; init; }
    public int ChildrenCount { get; init; }
    public bool HasDisability { get; init; }
    public string? DisabilityDetails { get; init; }
    public string? SponsorType { get; set; }
    public string? SponsorEmployerName { get; set; }
    public string? SponsorEmployerNumber { get; set; }
    public DateOnly? SponsorQidExpiry { get; set; }
    public FileRefDto? SponsorCard { get; set; }
    public string? ResidenceCountry { get; set; }
    public string PhoneNumber { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string? Address { get; set; }
    public ResidenceAddressDto? ResidenceAddress { get; set; }
    public string InterviewLocation { get; set; } = default!;
}
