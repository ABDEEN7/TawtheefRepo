using Cortex.Mediator.Commands;
using FluentValidation;

namespace Tawtheef.Application.Common.Behaviours;

public class ValidationBehaviour<TCommand, TResponse>(IEnumerable<IValidator<TCommand>> validators)
    : ICommandPipelineBehavior<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    public async Task<TResponse> Handle(TCommand request, CommandHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (validators.Any())
        {
            var validationResults = await Task.WhenAll(
                validators.Select(v =>
                    v.ValidateAsync(new ValidationContext<TCommand>(request), cancellationToken)));

            var failures = validationResults
                .Where(r => r.Errors.Any())
                .SelectMany(r => r.Errors)
                .ToList();

            if (failures.Count != 0)
                throw new ValidationException(failures);
        }

        return await next();
    }
}
