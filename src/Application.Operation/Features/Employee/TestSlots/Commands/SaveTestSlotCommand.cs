using Application.Operation.Features.Employee.TestSlots.DTOs;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.TestSlots.Commands;

public sealed record SaveTestSlotCommand(Guid? Id, CreateTestSlotDto TestSlot)
    : IRequest<IResult<SavedTestSlotDto>>;
