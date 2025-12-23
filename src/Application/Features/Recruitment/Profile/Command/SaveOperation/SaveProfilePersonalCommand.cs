using FluentResults;
using MediatR;
using Tawtheef.Application.Features.Recruitment.Profile.DTOs;

namespace Tawtheef.Application.Features.Recruitment.Profile.Command.SaveOperation;

public sealed record SaveProfilePersonalCommand(
    Guid UserId,
    SaveProfilePersonalRequest Request
) : IRequest<IResult<Unit>>;
