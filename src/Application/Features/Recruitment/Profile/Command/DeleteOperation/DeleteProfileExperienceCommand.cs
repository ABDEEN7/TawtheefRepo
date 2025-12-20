using FluentResults;
using MediatR;

namespace Tawtheef.Application.Features.Recruitment.Profile.Command.DeleteOperations;

public sealed record DeleteProfileExperienceCommand(Guid ExperienceId): IRequest<IResult<Unit>>;
