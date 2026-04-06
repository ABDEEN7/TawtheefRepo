using Tawtheef.Domain.Entities.MinisterOffice;

namespace Application.Operation.Features.Employee.MinisterOffice.DTOs;

public sealed class MinisterOfficeCandidateDto
{
    public Guid Id { get; init; }
    public string Qid { get; init; } = default!;
    public string FullNameEn { get; init; } = default!;
    public string FullNameAr { get; init; } = default!;
    public string PhoneNumber { get; init; } = default!;
    public string NationalityEn { get; init; } = default!;
    public string NationalityAr { get; init; } = default!;
    public bool IsFollowUpActive { get; init; }
    public MinisterOfficeCandidateStatus Status { get; init; }
    public string? GenderEn { get; init; }
    public string? GenderAr { get; init; }
    public string? CandidateTypeEn { get; init; }
    public string? CandidateTypeAr { get; init; }
    public string? TargetEntityEn { get; init; }
    public string? TargetEntityAr { get; init; }
    public bool IsPhoneNumberFromProfile { get; init; }
    public DateTime CreatedDate { get; init; }
}
