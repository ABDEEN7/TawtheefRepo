using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.TestSlots.Commands;

public sealed record AuthorizeTestSlotCandidateEntryCommand(Guid TestSlotId, Guid CandidateId) : IRequest<IResult<Unit>>;
