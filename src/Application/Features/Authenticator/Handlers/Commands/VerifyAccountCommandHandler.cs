using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.NotificationServices;
using Tawtheef.Application.Features.Authenticator.Commands;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Authenticator.Handlers.Commands;

public class VerifyAccountCommandHandler(IEmailService emailService, UserManager<User> userManager)
    : IRequestHandler<VerifyAccountCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(VerifyAccountCommand request, CancellationToken cancellationToken)
    {
        // Validate the token and email
        var user = await userManager.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken: cancellationToken);
        if (user == null)
            return Result.Failure<bool>(ErrorsCodes.UserNotFound);
        var result = await userManager.ConfirmEmailAsync(user, request.Token);
        if (!result.Succeeded)
            return Result.Failure<bool>(ErrorsCodes.InvalidTokenOrEmail);
        // Send a confirmation email
        await emailService.SendEmailVerificationEmailSuccessAsync(user.Email!, user.FirstName);
        
        return Result.Success(true);
    }
}