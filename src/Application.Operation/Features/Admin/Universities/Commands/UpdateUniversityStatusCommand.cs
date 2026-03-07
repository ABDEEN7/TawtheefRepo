using MediatR;
using FluentResults;

namespace Application.Operation.Features.Admin.Universities.Commands;

public sealed record UpdateUniversityStatusCommand(Guid UniversityId, bool IsActive) : IRequest<IResult<Unit>>;

