using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Features.Operations.Employee.OrganizationStructures.DTOs;

namespace Tawtheef.Application.Features.Operations.Employee.OrganizationStructures.Queries;

public sealed record GetSectorsPagedQuery : PaginatedRequest, IRequest<IResult<PaginatedResult<SectorDto>>>
{
    public string? Search { get; init; }
    public bool? IsActive { get; init; }
}
