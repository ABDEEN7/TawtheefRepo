
using FluentValidation;
using MediatR;
using Tawtheef.Application.Common.Exceptions;
using Tawtheef.Domain.Constants;
using ValidationException = Tawtheef.Application.Common.Exceptions.ValidationException;

namespace Tawtheef.Application.Common.Behaviours;

public class ValidationBehaviour<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!validators.Any())
            return await next(cancellationToken);

        var context = new ValidationContext<TRequest>(request);

        var results = await Task.WhenAll(
            validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = results
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Count != 0)
        {
            var errors = failures.Select(f => new ValidationError
            {
                Code = string.IsNullOrWhiteSpace(f.ErrorCode)
                    ? ErrorsCodes.ValidationError
                    : f.ErrorCode,
                Message = f.ErrorMessage,
                Field = f.PropertyName
            }).ToList();

            throw new ValidationException(errors);
        }

        return await next(cancellationToken);
    }
}
