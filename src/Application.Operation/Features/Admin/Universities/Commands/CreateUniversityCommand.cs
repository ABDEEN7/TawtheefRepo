using MediatR;
using FluentResults;
using Microsoft.AspNetCore.Http;

namespace Application.Operation.Features.Admin.Universities.Commands;

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
    IFormFile? LogoArFile,
    IFormFile? LogoEnFile) : IRequest<IResult<Guid>>;

