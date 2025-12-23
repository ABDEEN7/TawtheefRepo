using FluentResults;
using MediatR;
using Tawtheef.Application.Features.Recruitment.Profile.DTOs;

namespace Tawtheef.Application.Features.Recruitment.Profile.Command.ChangeRequestOperation;

public sealed record RequestProfileExperienceChangeCommand(
    Guid UserId,
    SaveProfileExperienceRequest Request
) : IRequest<IResult<Unit>>;

