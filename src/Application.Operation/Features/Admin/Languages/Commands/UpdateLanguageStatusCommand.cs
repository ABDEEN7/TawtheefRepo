using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Application.Operation.Features.Admin.Languages.Commands;

public sealed record UpdateLanguageStatusCommand(Guid LanguageId, bool IsActive) : ICommand<IResult<Unit>>;
