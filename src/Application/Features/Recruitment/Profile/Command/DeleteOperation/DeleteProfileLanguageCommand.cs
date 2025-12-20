using FluentResults;
using MediatR;

namespace Tawtheef.Application.Features.Recruitment.Profile.Command.DeleteOperations;

public sealed record DeleteProfileLanguageCommand(Guid LanguageId): IRequest<IResult<Unit>>;