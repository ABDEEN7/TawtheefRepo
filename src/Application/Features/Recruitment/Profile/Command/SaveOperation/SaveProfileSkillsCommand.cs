using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

using Tawtheef.Application.Features.Recruitment.Profile.DTOs;

namespace Tawtheef.Application.Features.Recruitment.Profile.Command.SaveOperation;

public sealed record SaveProfileSkillsCommand(
    Guid UserId,
    SaveProfileSkillsRequest Request
) : ICommand<IResult<Unit>>;

public sealed record SaveProfileLanguagesCommand(
    Guid UserId,
    SaveProfileLanguagesRequest Request
) : ICommand<IResult<Unit>>;
