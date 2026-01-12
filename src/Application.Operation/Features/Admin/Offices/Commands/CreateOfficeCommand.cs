using System.ComponentModel.DataAnnotations;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Application.Operation.Features.Admin.Offices.Commands;

public sealed record CreateOfficeCommand(
    string NameAr,
    string NameEn,
    Guid CountryId,
    IReadOnlyCollection<Guid> SupportedCountryIds,
    [EmailAddress] string AdminEmail) : ICommand<IResult<Guid>>;
