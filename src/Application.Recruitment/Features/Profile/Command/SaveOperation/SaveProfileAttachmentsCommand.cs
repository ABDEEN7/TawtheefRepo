using Application.Recruitment.Features.Profile.DTOs.SaveOperation;
using MediatR;
using FluentResults;

namespace Application.Recruitment.Features.Profile.Command.SaveOperation;
public sealed record SaveProfileAttachmentsCommand(
    Guid UserId,
    SaveProfileAttachmentsRequest Request
) : IRequest<IResult<Unit>>;

