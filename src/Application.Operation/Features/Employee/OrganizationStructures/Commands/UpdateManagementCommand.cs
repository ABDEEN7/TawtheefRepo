using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Application.Operation.Features.Employee.OrganizationStructures.Commands;

public sealed record UpdateManagementCommand(
    Guid Id,
    Guid SectorId,
    string NameAr,
    string NameEn,
    string? DescriptionAr,
    string? DescriptionEn,
    bool IsActive,
    int DisplayOrder) : ICommand<IResult<Unit>>;
