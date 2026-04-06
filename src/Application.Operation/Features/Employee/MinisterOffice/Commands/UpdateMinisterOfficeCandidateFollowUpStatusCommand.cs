using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.MinisterOffice.Commands;

public record UpdateMinisterOfficeCandidateFollowUpStatusCommand(Guid CandidateId, bool IsActive)
    : IRequest<IResult<Unit>>;
