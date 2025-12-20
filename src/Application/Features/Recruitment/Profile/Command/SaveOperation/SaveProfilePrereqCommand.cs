using FluentResults;
using MediatR;
using Tawtheef.Application.Features.Recruitment.Profile.DTOs;

namespace Tawtheef.Application.Features.Recruitment.Profile.Command.SaveOperations;

public sealed record SaveProfilePrereqCommand(
    Guid UserId,
    SaveProfilePrereqRequest Request
) : IRequest<IResult<Unit>>;
