using FluentResults;
using MediatR;
using Tawtheef.Application.Features.Recruitment.Profile.DTOs;

namespace Tawtheef.Application.Features.Recruitment.Profile.Command.SaveOperation;

public sealed record SaveProfileContactCommand(
    Guid UserId,
    SaveProfileContactRequest Request
) : IRequest<IResult<Unit>>;
