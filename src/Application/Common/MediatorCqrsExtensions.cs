using System.Reflection;
using Cortex.Mediator;
using Cortex.Mediator.Commands;
using Cortex.Mediator.Queries;

namespace Tawtheef.Application.Common;


public static class MediatorCqrsExtensions
{
     // Call-site: await mediator.Send(command, ct);
    extension(IMediator mediator)
    {
        public Task<TResponse> Send<TResponse>(ICommand<TResponse> command,
            CancellationToken ct = default)
            => InvokeTwoGenericMethod<TResponse>(mediator, methodName: "SendCommandAsync", request: command, ct);

        public Task<TResponse> Send<TResponse>(IQuery<TResponse> query,
            CancellationToken ct = default)
            => InvokeTwoGenericMethod<TResponse>(mediator, methodName: "SendQueryAsync", request: query, ct);
    }

    // Call-site: await mediator.Send(query, ct);

    /// <summary>
    /// Invokes the mediator overload that has two generic parameters:
    /// <c>SendCommandAsync&lt;TRequest, TResponse&gt;(TRequest, CancellationToken)</c>
    /// or
    /// <c>SendQueryAsync&lt;TRequest, TResponse&gt;(TRequest, CancellationToken)</c>.
    ///
    /// The method is closed using the request’s concrete runtime type (not the
    /// interface type) so DI resolves the correct handler:
    /// <c>ICommandHandler&lt;ConcreteRequest, TResponse&gt;</c>
    /// instead of mistakenly looking for
    /// <c>ICommandHandler&lt;ICommand&lt;TResponse&gt;, TResponse&gt;</c>.
    /// </summary>
    private static Task<TResponse> InvokeTwoGenericMethod<TResponse>(
        IMediator mediator,
        string methodName,
        object request,
        CancellationToken ct)
    {
        var mediatorType = mediator.GetType();
        var requestType = request.GetType();

        // Find the overload with exactly 2 generic args and (TRequest, CancellationToken)
        var method = mediatorType
            .GetMethods(BindingFlags.Instance | BindingFlags.Public)
            .FirstOrDefault(m =>
                m.Name == methodName &&
                m.IsGenericMethodDefinition &&
                m.GetGenericArguments().Length == 2 &&
                m.GetParameters().Length == 2 &&
                m.GetParameters()[0].ParameterType.IsGenericParameter &&
                m.GetParameters()[1].ParameterType == typeof(CancellationToken));

        if (method is null)
            throw new MissingMethodException(
                $"Could not find {methodName}<TRequest, TResponse>(TRequest, CancellationToken) on '{mediatorType.FullName}'.");

        var closed = method.MakeGenericMethod(requestType, typeof(TResponse));

        var result = closed.Invoke(mediator, [request, ct]);
        return result as Task<TResponse>
               ?? throw new InvalidOperationException(
                   $"{methodName} did not return Task<{typeof(TResponse).Name}> for request '{requestType.FullName}'.");
    }
}
