using Application.Recruitment.Features.Profile.DTOs.SaveOperation;
using MediatR;
using FluentResults;

namespace Application.Recruitment.Features.Profile.Command.RevisionOperation;

public sealed record ReviseProfileExperienceCommand(
    Guid UserId,
    SaveProfileExperienceRequest Request
) : IRequest<IResult<Unit>>;

