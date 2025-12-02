using FluentResults;
using MediatR;

namespace Tawtheef.Application.Features.Recruitment.Profile.Command;

public sealed record DeleteProfileTrainingCommand(Guid TrainingId): IRequest<IResult<Unit>>;