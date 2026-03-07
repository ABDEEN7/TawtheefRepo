using MediatR;
using FluentResults;

namespace Application.Recruitment.Features.Profile.Command.DeleteOperation;

public sealed record DeleteProfileAttachmentCommand(Guid UserId,Guid Id): IRequest<IResult<Unit>>;

