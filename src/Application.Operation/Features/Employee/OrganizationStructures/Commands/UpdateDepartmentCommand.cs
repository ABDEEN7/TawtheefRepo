using MediatR;
using FluentResults;

namespace Application.Operation.Features.Employee.OrganizationStructures.Commands;

public sealed record UpdateDepartmentCommand(
    Guid Id,
    Guid ManagementId,
    string NameAr,
    string NameEn,
    string? DescriptionAr,
    string? DescriptionEn,
    bool IsActive,
    int DisplayOrder) : IRequest<IResult<Unit>>;

