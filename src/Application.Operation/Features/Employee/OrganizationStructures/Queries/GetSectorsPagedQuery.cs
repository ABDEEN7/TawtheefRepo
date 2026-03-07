using Application.Operation.Features.Employee.OrganizationStructures.DTOs;
using MediatR;
using FluentResults;
using Tawtheef.Application.Common.Models.Pagination;

namespace Application.Operation.Features.Employee.OrganizationStructures.Queries;

public sealed record GetSectorsPagedQuery : PaginatedRequest, IRequest<IResult<PaginatedResult<SectorDto>>>
{
    public string? Search { get; init; }
    public bool? IsActive { get; init; }
}

