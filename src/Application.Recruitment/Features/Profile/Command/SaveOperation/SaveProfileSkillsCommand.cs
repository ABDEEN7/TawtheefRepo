using Application.Recruitment.Features.Profile.DTOs.SaveOperation;
using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Application.Recruitment.Features.Profile.Command.SaveOperation;

public sealed record SaveProfileSkillsCommand(
    Guid UserId,
    SaveProfileSkillsRequest Request
) : ICommand<IResult<Unit>>;

public sealed record SaveProfileLanguagesCommand(
    Guid UserId,
    SaveProfileLanguagesRequest Request
) : ICommand<IResult<Unit>>;
