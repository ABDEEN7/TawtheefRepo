using Application.Operation.Features.Employee.TestSlots.DTOs;
using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Models.Pagination;

namespace Application.Operation.Features.Employee.TestSlots.Queries;

public sealed record GetTestSlotsQuery
    : PaginatedRequest, IRequest<IResult<PaginatedResult<TestSlotListItemDto>>>
{
    public string? SearchTerm { get; init; }
    public Guid? RoomId { get; init; }
    public DateOnly? DateFrom { get; init; }
    public DateOnly? DateTo { get; init; }
}
