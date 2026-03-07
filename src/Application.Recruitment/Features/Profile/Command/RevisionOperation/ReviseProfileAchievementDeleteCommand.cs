using MediatR;
using FluentResults;

namespace Application.Recruitment.Features.Profile.Command.RevisionOperation;

public sealed record ReviseProfileAchievementDeleteCommand(Guid UserId,Guid Id): IRequest<IResult<Unit>>;

