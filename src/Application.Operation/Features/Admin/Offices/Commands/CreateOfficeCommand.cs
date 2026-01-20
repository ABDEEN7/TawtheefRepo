using System.ComponentModel.DataAnnotations;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Application.Operation.Features.Admin.Offices.Commands;

public sealed record CreateOfficeCommand(
    string NameAr,
    string NameEn,
    Guid CountryId,
    IReadOnlyCollection<Guid> SupportedCountryIds,
    [Required] string AdminNameAr,
    [Required] string AdminNameEn,
    [EmailAddress] string AdminEmail,
    [Required] string PhoneCountryCode,
    [Required] string PhoneNumber) : ICommand<IResult<Guid>>;
