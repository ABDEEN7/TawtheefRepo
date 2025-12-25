using System.ComponentModel.DataAnnotations;
using FluentResults;
using MediatR;
using Tawtheef.Application.Features.Operations.Admin.Offices.DTOs;

namespace Tawtheef.Application.Features.Operations.Admin.Offices.Commands;

public sealed record CreateOfficeCommand(
    string NameAr,
    string NameEn,
    Guid CountryId,
    IReadOnlyCollection<Guid> SupportedCountryIds,
    [EmailAddress] string AdminEmail) : IRequest<IResult<Guid>>;
