using Microsoft.AspNetCore.Http;

namespace Tawtheef.Application.Features.Recruitment.Profile.DTOs;

public sealed class SaveProfilePersonalRequest
{
    public bool Submit { get; set; }

    public string? FullNameAr { get; set; }
    public string? FullNameEn { get; set; }
    public string? NationalNumber { get; set; }
    public DateOnly? QIDExpiry { get; set; }
    public DateOnly? BirthDate { get; set; }

    public Guid? NationalityId { get; set; }
    public Guid? GenderId { get; set; }
    public Guid? ReligionId { get; set; }
    public Guid? MaritalStatusId { get; set; }
    public int? ChildrenCount { get; set; }

    public bool HasDisability { get; set; }
    public string? DisabilityDetails { get; set; }

    public Guid? SponsorTypeId { get; set; }
    public string? SponsorEmployerName { get; set; }
    public string? SponsorEmployerNumber { get; set; }
    public DateOnly? SponsorCardExpiryData { get; set; }
    public IFormFile? SponsorCard { get; set; }
    public string? SponsorCardFileName { get; set; }
}
