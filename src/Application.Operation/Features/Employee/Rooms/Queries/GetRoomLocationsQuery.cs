using Application.Operation.Features.Employee.Rooms.DTOs;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Rooms.Queries;

public sealed record GetRoomLocationsQuery : IRequest<IResult<List<RoomLocationDto>>>;
