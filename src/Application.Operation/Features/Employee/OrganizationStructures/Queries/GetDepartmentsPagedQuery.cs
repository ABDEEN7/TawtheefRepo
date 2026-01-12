using Application.Operation.Features.Employee.OrganizationStructures.DTOs;
using Cortex.Mediator.Queries;
using FluentResults;
using Tawtheef.Application.Common.Models.Pagination;

namespace Application.Operation.Features.Employee.OrganizationStructures.Queries;

public sealed record GetDepartmentsPagedQuery : PaginatedRequest, IQuery<IResult<PaginatedResult<DepartmentDto>>>
{
    public string? Search { get; init; }
    public bool? IsActive { get; init; }
    public Guid? SectorId { get; init; }
    public Guid? ManagementId { get; init; }
}
