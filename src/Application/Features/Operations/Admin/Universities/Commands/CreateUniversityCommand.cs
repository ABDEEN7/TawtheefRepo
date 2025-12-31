using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Tawtheef.Application.Features.Operations.Admin.Universities.Commands;

public sealed record CreateUniversityCommand(
    string NameAr,
    string NameEn,
    string? DescriptionAr,
    string? DescriptionEn,
    Guid CountryId,
    Guid CityId,
    string? WebSite,
    string? Phone,
    string? Email,
    string? Code,
    string? OriginalName,
    bool IsActive,
    int? LogoArFileIndex,
    int? LogoEnFileIndex,
    List<IFormFile> Files) : IRequest<IResult<Guid>>;
