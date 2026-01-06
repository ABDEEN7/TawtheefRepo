using System.ComponentModel.DataAnnotations;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Tawtheef.Application.Features.Operations.Admin.Offices.Commands;

public sealed record UpdateOfficeCommand(
    Guid Id,
    string NameAr,
    string NameEn,
    [EmailAddress] string AdminEmail,
    IReadOnlyCollection<Guid> SupportedCountryIds) : ICommand<IResult<bool>>;
