using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Application.Recruitment.Features.Profile.DTOs.SaveOperation;

public sealed class SaveProfileContactRequest
{
    public bool Submit { get; set; }

    public Guid ResidenceCountryId { get; set; }

    [RegularExpression(@"^[a-zA-Z0-9\u0600-\u06FF\s\,\.\-]+$", ErrorMessage = "Address contains invalid characters.")]
    public string? Address { get; set; }
    public Guid InterviewLocationId { get; set; }

    public NationalAddressDto? NationalAddress { get; set; }
}

public sealed class NationalAddressDto
{
    public int Zone { get; set; }
    public int Street { get; set; }
    public int Building { get; set; }
    public int Unit { get; set; }

    [RegularExpression(@"^[a-zA-Z0-9\s\,\.\-\(\)]+$", ErrorMessage = "File name contains invalid characters.")]
    public string? NationalAddressFileName { get; set; }
    public IFormFile? NationalAddress { get; set; }
}

