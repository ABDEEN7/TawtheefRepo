using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Application.Operation.Features.Admin.HomeContent.Faqs.Commands;

public sealed record UpdateFaqStatusCommand(Guid FaqId, bool IsActive) : ICommand<IResult<Unit>>;
