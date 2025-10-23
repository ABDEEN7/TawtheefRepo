using System;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Tawtheef.Application.Features.Authenticator.Commands;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Authenticator.Handlers.Commands;

public class ForgetPasswordHandler(
    UserManager<User> userManager,
    IMediator mediator,
    TimeProvider time
) : IRequestHandler<ForgetPasswordCommand, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(ForgetPasswordCommand request, CancellationToken ct)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null)
            return Result.Failure<Unit>(ErrorsCodes.UserNotFound);

        // Ask domain to validate and raise the event (no token here)
        var now = time.GetLocalNow().UtcDateTime;
        var res = user.RequestPasswordReset(now);
        if (res.IsFailure) return Result.Failure<Unit>(res.Error);

        // Publish inline because Identity may use a different DbContext
        foreach (var ev in user.DomainEvents) await mediator.Publish(ev, ct);
        user.ClearDomainEvents();

        return Result.Success(Unit.Value);
    }
}