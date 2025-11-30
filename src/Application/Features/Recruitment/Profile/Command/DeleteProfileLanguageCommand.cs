using FluentResults;
using MediatR;

namespace Tawtheef.Application.Features.Recruitment.Profile.Command;

public sealed record DeleteProfileLanguageCommand(Guid LanguageId): IRequest<IResult<Unit>>;