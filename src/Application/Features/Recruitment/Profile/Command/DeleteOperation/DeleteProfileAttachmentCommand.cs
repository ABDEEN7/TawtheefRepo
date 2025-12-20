using FluentResults;
using MediatR;

namespace Tawtheef.Application.Features.Recruitment.Profile.Command.DeleteOperations;

public sealed record DeleteProfileAttachmentCommand(Guid AttachmentId): IRequest<IResult<Unit>>;
