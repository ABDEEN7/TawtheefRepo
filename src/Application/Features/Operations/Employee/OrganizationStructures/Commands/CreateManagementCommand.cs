using Cortex.Mediator.Commands;
using FluentResults;

namespace Tawtheef.Application.Features.Operations.Employee.OrganizationStructures.Commands;

public sealed record CreateManagementCommand(
    Guid SectorId,
    string NameAr,
    string NameEn,
    string? DescriptionAr,
    string? DescriptionEn,
    bool IsActive,
    int DisplayOrder) : ICommand<IResult<Guid>>;
