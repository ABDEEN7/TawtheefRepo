using Cortex.Mediator.Commands;
using FluentResults;

namespace Application.Operation.Features.Admin.HomeContent.Faqs.Commands;

public sealed record DeleteFaqCommand(Guid FaqId) : ICommand<IResult<Unit>>;
