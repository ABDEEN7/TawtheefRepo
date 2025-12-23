using FluentResults;
using MediatR;

namespace Tawtheef.Application.Features.Recruitment.Profile.Command.DeleteOperation;

public sealed record DeleteProfileAttachmentCommand(Guid AttachmentId): IRequest<IResult<Unit>>;
