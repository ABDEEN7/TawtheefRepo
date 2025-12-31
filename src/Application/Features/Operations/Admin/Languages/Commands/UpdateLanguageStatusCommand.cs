using FluentResults;
using MediatR;

namespace Tawtheef.Application.Features.Operations.Admin.Languages.Commands;

public sealed record UpdateLanguageStatusCommand(Guid LanguageId, bool IsActive) : IRequest<IResult<Unit>>;
