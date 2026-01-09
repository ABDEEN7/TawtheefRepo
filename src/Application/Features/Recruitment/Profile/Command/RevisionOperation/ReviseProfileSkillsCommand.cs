using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

using Tawtheef.Application.Features.Recruitment.Profile.DTOs;

namespace Tawtheef.Application.Features.Recruitment.Profile.Command.RevisionOperation;

public sealed record ReviseProfileSkillsCommand(
    Guid UserId,
    SaveProfileSkillsRequest Request
) : ICommand<IResult<Unit>>;

public sealed record ReviseProfileLanguagesCommand(
    Guid UserId,
    SaveProfileLanguagesRequest Request
) : ICommand<IResult<Unit>>;
