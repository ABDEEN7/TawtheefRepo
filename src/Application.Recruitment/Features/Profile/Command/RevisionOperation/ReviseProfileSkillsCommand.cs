using Application.Recruitment.Features.Profile.DTOs.SaveOperation;
using MediatR;
using FluentResults;

namespace Application.Recruitment.Features.Profile.Command.RevisionOperation;

public sealed record ReviseProfileSkillsCommand(
    Guid UserId,
    SaveProfileSkillsRequest Request
) : IRequest<IResult<Unit>>;

public sealed record ReviseProfileLanguagesCommand(
    Guid UserId,
    SaveProfileLanguagesRequest Request
) : IRequest<IResult<Unit>>;

