namespace Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals.DTOs.Spanshot;

public sealed record PersonalSnapshot
{
    public string? FullNameAr { get; init; }
    public string? FullNameEn { get; init; }
    public string? NationalNumber { get; init; }
    public DateOnly? QidExpiry { get; init; }
    public DateOnly? BirthDate { get; init; }
    public Guid? NationalityId { get; init; }
    public Guid? GenderId { get; init; }
    public Guid? ReligionId { get; init; }
    public Guid? MaritalStatusId { get; init; }
    public int? ChildrenCount { get; init; }
    public bool HasDisability { get; init; }
    public string? DisabilityDetails { get; init; }
    public Guid? SponsorTypeId { get; init; }
    public string? SponsorEmployerName { get; init; }
    public string? SponsorEmployerNumber { get; init; }
    public DateOnly? SponsorQidExpiry { get; init; }
    public Guid? SponsorCardResourceId { get; init; }
}