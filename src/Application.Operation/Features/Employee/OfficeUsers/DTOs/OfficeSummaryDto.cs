namespace Application.Operation.Features.Employee.OfficeUsers.DTOs;

public sealed record OfficeSummaryDto
{
    public string NameAr { get; init; } = string.Empty;
    public string NameEn { get; init; } = string.Empty;
    public string CountryNameAr { get; init; } = string.Empty;
    public string CountryNameEn { get; init; } = string.Empty;
    public string? PhoneCountryCode { get; init; }
    public string? PhoneNumber { get; init; }
}
