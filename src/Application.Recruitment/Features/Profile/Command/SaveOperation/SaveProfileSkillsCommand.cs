using Application.Recruitment.Features.Profile.DTOs.SaveOperation;
using MediatR;
using FluentResults;

namespace Application.Recruitment.Features.Profile.Command.SaveOperation;

public sealed record SaveProfileSkillsCommand(
    Guid UserId,
    SaveProfileSkillsRequest Request
) : IRequest<IResult<Unit>>;

public sealed record SaveProfileLanguagesCommand(
    Guid UserId,
    SaveProfileLanguagesRequest Request
) : IRequest<IResult<Unit>>;

