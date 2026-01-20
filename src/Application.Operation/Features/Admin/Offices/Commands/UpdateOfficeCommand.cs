using System.ComponentModel.DataAnnotations;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Application.Operation.Features.Admin.Offices.Commands;

public sealed record UpdateOfficeCommand(
    Guid Id,
    string NameAr,
    string NameEn,
    [Required] string AdminNameAr,
    [Required] string AdminNameEn,
    [EmailAddress] string AdminEmail,
    [Required] string PhoneCountryCode,
    [Required] string PhoneNumber,
    IReadOnlyCollection<Guid> SupportedCountryIds) : ICommand<IResult<bool>>;
