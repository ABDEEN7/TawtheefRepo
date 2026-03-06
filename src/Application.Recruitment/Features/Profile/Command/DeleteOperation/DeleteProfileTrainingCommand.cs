using MediatR;
using FluentResults;

namespace Application.Recruitment.Features.Profile.Command.DeleteOperation;

public sealed record DeleteProfileTrainingCommand(Guid UserId,Guid Id): IRequest<IResult<Unit>>;

