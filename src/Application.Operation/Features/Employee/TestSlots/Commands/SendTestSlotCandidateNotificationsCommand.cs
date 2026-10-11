using Application.Operation.Features.Employee.TestSlots.DTOs;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.TestSlots.Commands;

public sealed record SendTestSlotCandidateNotificationsCommand(Guid TestSlotId, Guid? TestSessionId)
    : IRequest<IResult<SendTestSlotCandidateNotificationsResultDto>>;
