using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Tawtheef.Application.Features.Authenticator.Commands;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Authenticator.Handlers.Commands;

// VerifyLockPasswordHandler.cs
public class VerifyLockPasswordHandler(UserManager<User> userManager)
    : IRequestHandler<VerifyLockPasswordCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(VerifyLockPasswordCommand request, CancellationToken cancellationToken)
    {
        // Find the user
        var user = await userManager.FindByIdAsync(request.UserId.ToString());
        if (user == null)
            return Result.Failure<bool>(ErrorsCodes.UserNotFound);

        // Verify password
        var isValid = await userManager.CheckPasswordAsync(user, request.Password);
        return !isValid ? Result.Failure<bool>(ErrorsCodes.InvalidPassword) : Result.Success(true);
    }
}
