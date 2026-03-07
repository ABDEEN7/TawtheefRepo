using MediatR;
using FluentResults;

namespace Application.Recruitment.Features.Profile.Command.RevisionOperation;

public sealed record ReviseProfileSkillDeleteCommand(Guid UserId, Guid Id): IRequest<IResult<Unit>>;

