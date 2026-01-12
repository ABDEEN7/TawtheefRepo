using Application.Operation.Features.Employee.OrganizationStructures.DTOs;
using Cortex.Mediator.Queries;
using FluentResults;
using Tawtheef.Application.Common.Models.Pagination;

namespace Application.Operation.Features.Employee.OrganizationStructures.Queries;

public sealed record GetManagementsPagedQuery : PaginatedRequest, IQuery<IResult<PaginatedResult<ManagementDto>>>
{
    public string? Search { get; init; }
    public bool? IsActive { get; init; }
    public Guid? SectorId { get; init; }
}
