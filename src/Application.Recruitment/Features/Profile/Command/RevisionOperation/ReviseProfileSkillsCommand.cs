using Application.Recruitment.Features.Profile.DTOs.SaveOperation;
using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Application.Recruitment.Features.Profile.Command.RevisionOperation;

public sealed record ReviseProfileSkillsCommand(
    Guid UserId,
    SaveProfileSkillsRequest Request
) : ICommand<IResult<Unit>>;

public sealed record ReviseProfileLanguagesCommand(
    Guid UserId,
    SaveProfileLanguagesRequest Request
) : ICommand<IResult<Unit>>;
