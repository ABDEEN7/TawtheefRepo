using System.ComponentModel.DataAnnotations;
using Application.Recruitment.Common.ModelBinding;
using Application.Recruitment.Common.Validation;
using Microsoft.AspNetCore.Http;

namespace Application.Recruitment.Features.Profile.DTOs.SaveOperation;

public sealed class SaveProfilePersonalRequest
{
    public bool Submit { get; set; }

    [NormalizeText]
    [RegularExpression(InputValidationPatterns.Textbox, ErrorMessage = "Full name contains invalid characters.")]
    public string? FullNameAr { get; set; }

    [NormalizeText]
    [RegularExpression(InputValidationPatterns.Textbox, ErrorMessage = "Full name contains invalid characters.")]
    public string? FullNameEn { get; set; }

    [RegularExpression(InputValidationPatterns.NationalNumber, ErrorMessage = "National number contains invalid characters.")]
    public string? NationalNumber { get; set; }
    public DateOnly? QIDExpiry { get; set; }
    public DateOnly? BirthDate { get; set; }

    public Guid? NationalityId { get; set; }
    public Guid? GenderId { get; set; }
    public Guid? ReligionId { get; set; }
    public Guid? MaritalStatusId { get; set; }
    public int? ChildrenCount { get; set; }

    public bool HasDisability { get; set; }

    [StringLength(500)]
    [RegularExpression(InputValidationPatterns.TextArea, ErrorMessage = "Disability details contains invalid characters.")]
    public string? DisabilityDetails { get; set; }

    public Guid? SponsorTypeId { get; set; }

    [RegularExpression(InputValidationPatterns.Textbox, ErrorMessage = "Sponsor employer name contains invalid characters.")]
    public string? SponsorEmployerName { get; set; }

    [RegularExpression(@"^[0-9]+$", ErrorMessage = "Sponsor employer number contains invalid characters.")]
    public string? SponsorEmployerNumber { get; set; }
    public DateOnly? SponsorQidExpiry { get; set; }
    public IFormFile? SponsorCard { get; set; }

    [RegularExpression(@"^[^\\/:*?""<>|]+$", ErrorMessage = "File name contains invalid characters.")]
    public string? SponsorCardFileName { get; set; }
}
