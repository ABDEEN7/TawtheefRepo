using FluentResults;
using MediatR;

namespace Tawtheef.Application.Features.Recruitment.Profile.Command.DeleteOperation;

public sealed record DeleteProfileSkillCommand(Guid SkillId): IRequest<IResult<Unit>>;
