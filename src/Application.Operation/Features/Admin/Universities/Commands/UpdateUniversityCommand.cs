using Cortex.Mediator.Commands;
using FluentResults;
using Microsoft.AspNetCore.Http;

namespace Application.Operation.Features.Admin.Universities.Commands;

public sealed record UpdateUniversityCommand(
    Guid Id,
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
    List<IFormFile> Files) : ICommand<IResult<Guid>>;
