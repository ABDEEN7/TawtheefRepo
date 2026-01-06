using Cortex.Mediator.Queries;
using FluentResults;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Features.Operations.Employee.OrganizationStructures.DTOs;

namespace Tawtheef.Application.Features.Operations.Employee.OrganizationStructures.Queries;

public sealed record GetManagementsPagedQuery : PaginatedRequest, IQuery<IResult<PaginatedResult<ManagementDto>>>
{
    public string? Search { get; init; }
    public bool? IsActive { get; init; }
    public Guid? SectorId { get; init; }
}
