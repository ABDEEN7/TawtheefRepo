using Application.Operation.Features.Employee.TestSlots.DTOs;
using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Models.Pagination;

namespace Application.Operation.Features.Employee.TestSlots.Queries;

public sealed record GetTestSlotCandidatesQuery
    : PaginatedRequest, IRequest<IResult<PaginatedResult<TestSlotCandidateListItemDto>>>
{
    public Guid TestSlotId { get; init; }
    public string? Search { get; init; }
    public Guid? SessionId { get; init; }
    public Guid? AttendanceStatusId { get; init; }
}
