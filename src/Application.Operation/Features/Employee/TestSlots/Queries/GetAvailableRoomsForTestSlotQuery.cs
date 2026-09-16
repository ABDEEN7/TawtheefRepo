using FluentResults;
using MediatR;
using Application.Operation.Features.Employee.Rooms.DTOs;

namespace Application.Operation.Features.Employee.TestSlots.Queries;

public sealed record GetAvailableRoomsForTestSlotQuery(string Language) : IRequest<IResult<List<RoomDto>>>;
