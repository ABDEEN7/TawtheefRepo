using FluentResults;
using MediatR;
using Tawtheef.Application.Features.Recruitment.Profile.DTOs;

namespace Tawtheef.Application.Features.Recruitment.Profile.Command.SaveOperation;

public sealed record SaveProfileEducationCommand(
    Guid UserId,
    SaveProfileEducationRequest Request
) : IRequest<IResult<Unit>>;
