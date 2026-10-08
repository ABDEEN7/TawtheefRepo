using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.TestSlots.Commands;

public sealed record MarkTestSlotCandidatePresentCommand(Guid TestSlotId, Guid CandidateId) : IRequest<IResult<Unit>>;
