using FluentResults;
using MediatR;

namespace Tawtheef.Application.Features.Recruitment.Profile.Command;

public sealed record DeleteProfileSkillCommand(Guid SkillId): IRequest<IResult<Unit>>;
