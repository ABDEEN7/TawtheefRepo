using FluentResults;
using MediatR;
using Tawtheef.Application.Features.Recruitment.Profile.DTOs;

namespace Tawtheef.Application.Features.Recruitment.Profile.Command.ChangeRequestOperation;

public sealed record RequestProfileLanguagesChangeCommand(
    Guid UserId,
    SaveProfileLanguagesRequest Request
) : IRequest<IResult<Unit>>;

