namespace Application.Operation.Features.Employee.Locations.DTOs;

public sealed record LocationDto
{
    public Guid Id { get; init; }
    public required string NameAr { get; init; }
    public string? NameEn { get; init; }
    public required string LocationLink { get; init; }
    public string? Notes { get; init; }
}
