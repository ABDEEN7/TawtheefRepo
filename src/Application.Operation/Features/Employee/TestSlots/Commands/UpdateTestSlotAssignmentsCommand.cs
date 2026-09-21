using Application.Operation.Features.Employee.TestSlots.DTOs;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.TestSlots.Commands;

public sealed record UpdateTestSlotAssignmentsCommand(Guid TestSlotId, UpdateTestSlotAssignmentsDto Assignments)
    : IRequest<IResult<Unit>>;
