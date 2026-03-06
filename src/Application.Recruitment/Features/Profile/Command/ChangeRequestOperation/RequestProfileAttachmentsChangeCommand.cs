using Application.Recruitment.Features.Profile.DTOs.SaveOperation;
using MediatR;
using FluentResults;

namespace Application.Recruitment.Features.Profile.Command.ChangeRequestOperation;

public sealed record RequestProfileAttachmentsChangeCommand(
    Guid UserId,
    SaveProfileAttachmentsRequest Request
) : IRequest<IResult<Unit>>;


