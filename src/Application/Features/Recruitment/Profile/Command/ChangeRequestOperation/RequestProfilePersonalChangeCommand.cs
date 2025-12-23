using FluentResults;
using MediatR;
using Tawtheef.Application.Features.Recruitment.Profile.DTOs;

namespace Tawtheef.Application.Features.Recruitment.Profile.Command.ChangeRequestOperation;

public sealed record RequestProfilePersonalChangeCommand(
    Guid UserId,
    SaveProfilePersonalRequest Request
) : IRequest<IResult<Unit>>;

