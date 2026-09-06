namespace Application.Operation.Features.Employee.Locations.DTOs;

public sealed record SaveLocationDto(
    string NameAr,
    string? NameEn,
    string LocationLink,
    string? Notes);
