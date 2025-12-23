using FluentResults;
using MediatR;

namespace Tawtheef.Application.Features.Recruitment.Profile.Command.DeleteOperation;

public sealed record DeleteProfileExperienceCommand(Guid ExperienceId): IRequest<IResult<Unit>>;
