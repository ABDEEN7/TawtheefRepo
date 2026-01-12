using Microsoft.AspNetCore.Http;

namespace Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals.DTOs.SaveOperation;

public sealed class SaveProfileContactRequest
{
    public bool Submit { get; set; }

    public Guid ResidenceCountryId { get; set; }
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
    public string? NationalAddressFileName { get; set; }
    public IFormFile? NationalAddress { get; set; }
}
