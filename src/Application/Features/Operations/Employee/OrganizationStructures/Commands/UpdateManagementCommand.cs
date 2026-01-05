using FluentResults;
using MediatR;

namespace Tawtheef.Application.Features.Operations.Employee.OrganizationStructures.Commands;

public sealed record UpdateManagementCommand(
    Guid Id,
    Guid SectorId,
    string NameAr,
    string NameEn,
    string? DescriptionAr,
    string? DescriptionEn,
    bool IsActive,
    int DisplayOrder) : IRequest<IResult<Unit>>;
