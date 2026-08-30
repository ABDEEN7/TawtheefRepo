using Application.Operation.Features.Employee.Rooms.DTOs;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Rooms.Commands;

public sealed record CreateRoomCommand(CreateRoomDto Room) : IRequest<IResult<Guid>>;
