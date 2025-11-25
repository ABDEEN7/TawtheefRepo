using FluentResults;
using MediatR;
using Tawtheef.Application.Features.Recruitment.Profile.DTOs;

namespace Tawtheef.Application.Features.Recruitment.Profile.Command;

public sealed record SaveProfileEducationCommand(
    Guid UserId,
    SaveProfileEducationRequest Request
) : IRequest<IResult<Unit>>;
