using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.TestSlots.Commands;

public sealed record CloseTestSlotCommand(Guid TestSlotId) : IRequest<IResult<Unit>>;
