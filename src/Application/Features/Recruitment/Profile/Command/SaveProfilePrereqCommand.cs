using FluentResults;
using MediatR;
using Tawtheef.Application.Features.Recruitment.Profile.DTOs;

namespace Tawtheef.Application.Features.Recruitment.Profile.Command;

public sealed record SaveProfilePrereqCommand(
    Guid UserId,
    SaveProfilePrereqRequest Request
) : IRequest<IResult<Unit>>;
