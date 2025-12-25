using System.ComponentModel.DataAnnotations;
using FluentResults;
using MediatR;

namespace Tawtheef.Application.Features.Operations.Admin.Offices.Commands;

public sealed record UpdateOfficeCommand(
    Guid Id,
    string NameAr,
    string NameEn,
    [EmailAddress] string AdminEmail,
    IReadOnlyCollection<Guid> SupportedCountryIds) : IRequest<IResult<bool>>;
