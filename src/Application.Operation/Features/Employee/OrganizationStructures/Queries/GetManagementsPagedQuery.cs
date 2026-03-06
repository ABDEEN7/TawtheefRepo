using Application.Operation.Features.Employee.OrganizationStructures.DTOs;
using MediatR;
using FluentResults;
using Tawtheef.Application.Common.Models.Pagination;

namespace Application.Operation.Features.Employee.OrganizationStructures.Queries;

public sealed record GetManagementsPagedQuery : PaginatedRequest, IRequest<IResult<PaginatedResult<ManagementDto>>>
{
    public string? Search { get; init; }
    public bool? IsActive { get; init; }
    public Guid? SectorId { get; init; }
}

