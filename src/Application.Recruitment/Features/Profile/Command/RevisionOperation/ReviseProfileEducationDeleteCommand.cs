using MediatR;
using FluentResults;

namespace Application.Recruitment.Features.Profile.Command.RevisionOperation;

public sealed record ReviseProfileEducationDeleteCommand(Guid UserId,Guid Id): IRequest<IResult<Unit>>;

