using FluentResults;
using MediatR;

namespace Tawtheef.Application.Features.Recruitment.Profile.Command.DeleteOperation;

public sealed record DeleteProfileLanguageCommand(Guid LanguageId): IRequest<IResult<Unit>>;