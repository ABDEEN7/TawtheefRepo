using FluentResults;
using MediatR;
using Tawtheef.Application.Features.Recruitment.Profile.DTOs;

namespace Tawtheef.Application.Features.Recruitment.Profile.Command.SaveOperation;

public sealed record SaveProfileSkillsCommand(
    Guid UserId,
    SaveProfileSkillsRequest Request
) : IRequest<IResult<Unit>>;

public sealed record SaveProfileLanguagesCommand(
    Guid UserId,
    SaveProfileLanguagesRequest Request
) : IRequest<IResult<Unit>>;
