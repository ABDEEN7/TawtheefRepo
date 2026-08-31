using Application.Operation.Features.Employee.Rooms.DTOs;
using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Models.Pagination;

namespace Application.Operation.Features.Employee.Rooms.Queries;

public sealed record ListRoomsQuery : PaginatedRequest, IRequest<IResult<PaginatedResult<RoomDto>>>
{
    public string? Search { get; init; }
    public Guid? LocationId { get; init; }
    public Guid? RoomTypeId { get; init; }
    public Guid? StatusId { get; init; }
}
