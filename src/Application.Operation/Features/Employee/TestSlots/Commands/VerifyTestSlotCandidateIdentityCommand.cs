using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.TestSlots.Commands;

public sealed record VerifyTestSlotCandidateIdentityCommand(Guid TestSlotId, Guid CandidateId, bool Matched, string? Notes) : IRequest<IResult<Unit>>;
