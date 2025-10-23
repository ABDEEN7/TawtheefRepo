using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Tawtheef.Application.Features.Authenticator.Commands;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Authenticator.Handlers.Commands;

public class ResetPasswordHandler(UserManager<User> userManager) : IRequestHandler<ResetPasswordCommand, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return Result.Failure<Unit>(ErrorsCodes.UserNotFound);

        var result = await userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);

        return result.Succeeded
            ? Result.Success(Unit.Value)
            : Result.Failure<Unit>(ErrorsCodes.PasswordResetFailed);
    }
}

