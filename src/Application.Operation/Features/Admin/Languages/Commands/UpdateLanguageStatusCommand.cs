using MediatR;
using FluentResults;

namespace Application.Operation.Features.Admin.Languages.Commands;

public sealed record UpdateLanguageStatusCommand(Guid LanguageId, bool IsActive) : IRequest<IResult<Unit>>;

