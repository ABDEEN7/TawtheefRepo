using FluentResults;
using MediatR;
using Tawtheef.Application.Features.Recruitment.Profile.DTOs;

namespace Tawtheef.Application.Features.Recruitment.Profile.Command.ChangeRequestOperation;

public sealed record RequestProfileAttachmentsChangeCommand(
    Guid UserId,
    SaveProfileAttachmentsRequest Request
) : IRequest<IResult<Unit>>;

