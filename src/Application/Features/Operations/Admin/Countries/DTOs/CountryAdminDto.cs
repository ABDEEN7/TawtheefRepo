using System;

namespace Tawtheef.Application.Features.Operations.Admin.Countries.DTOs;

public sealed record CountryAdminDto
{
    public Guid Id { get; init; }
    public required string NameAr { get; init; }
    public required string NameEn { get; init; }
    public int Code { get; init; }
    public required string ISOCode { get; init; }
    public required string CodeAlpha { get; init; }
    public bool IsActive { get; init; }
}
