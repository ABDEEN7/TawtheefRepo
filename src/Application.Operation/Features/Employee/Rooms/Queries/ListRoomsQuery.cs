using Application.Operation.Features.Employee.Rooms.DTOs;
using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Domain.Enums.Rooms;

namespace Application.Operation.Features.Employee.Rooms.Queries;

public sealed record ListRoomsQuery : PaginatedRequest, IRequest<IResult<PaginatedResult<RoomDto>>>
{
    public string? Search { get; init; }
    public string? Location { get; init; }
    public RoomType? RoomType { get; init; }
    public RoomStatus? Status { get; init; }
}
