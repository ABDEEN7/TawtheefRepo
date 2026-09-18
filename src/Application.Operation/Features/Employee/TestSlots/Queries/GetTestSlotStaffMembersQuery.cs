using Application.Operation.Features.Employee.TestSlots.DTOs;
using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Models.Pagination;

namespace Application.Operation.Features.Employee.TestSlots.Queries;

public sealed record GetTestSlotStaffMembersQuery
    : PaginatedRequest, IRequest<IResult<PaginatedResult<TestSlotStaffMemberDto>>>
{
    public string? Search { get; init; }
    public bool? IsBlocked { get; init; }
}
