using Application.Recruitment.Features.Authenticator.Commands.Verification;
using Application.Recruitment.Features.Profile.Policies;
using MediatR;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Auth;
using Tawtheef.Domain.Entities.Users;

namespace Application.Recruitment.Features.Authenticator.Handlers.Commands.VerificationHandler;

public class ConfirmPhoneVerificationCommandHandler(
    IUnitOfWork unitOfWork,
    UserManager<User> userManager,
    TimeProvider timeProvider)
    : IRequestHandler<ConfirmPhoneVerificationCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(ConfirmPhoneVerificationCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
        if (user is null)
            return Result.Fail<Unit>(ErrorsCodes.UserNotFound);

        if (await IsLockedIdentityProviderAsync(user))
            return Result.Fail<Unit>(ErrorsCodes.UnauthorizedAction);

        var now = timeProvider.GetUtcNow().UtcDateTime;

        var verification = await unitOfWork.GetEntityRepository<ContactVerification>().DbSet
            .Where(v =>
                v.UserId == request.UserId &&
                v.Type == ContactVerificationType.Phone &&
                v.Destination == request.PhoneE164 &&
                !v.UsedAt.HasValue &&
                v.ExpiresAt >= now)
            .OrderByDescending(v => v.CreatedDate)
            .FirstOrDefaultAsync(cancellationToken);

        if (verification is null || !string.Equals(verification.Code, request.Code, StringComparison.Ordinal))
            return Result.Fail<Unit>(ErrorsCodes.InvalidVerificationCode);

        verification.UsedAt = now;
        
        user.PhoneNumber = request.PhoneE164;
        user.PhoneNumberConfirmed = true;

        await userManager.UpdateAsync(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(Unit.Value);
    }

    private async Task<bool> IsLockedIdentityProviderAsync(User user)
    {
        var logins = await userManager.GetLoginsAsync(user);
        return logins.Any(x => VerifiedIdentityProviders.IsLockedProvider(x.LoginProvider));
    }
}

