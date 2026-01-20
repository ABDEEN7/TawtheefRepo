using Application.Recruitment.Features.Authenticator.Commands.Verification;
using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Auth;
using Tawtheef.Domain.Entities.Users;

namespace Application.Recruitment.Features.Authenticator.Handlers.Commands.VerificationHandler;

public class ConfirmEmailVerificationCommandHandler(
    IUnitOfWork unitOfWork,
    UserManager<User> userManager)
    : ICommandHandler<ConfirmEmailVerificationCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(ConfirmEmailVerificationCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
        if (user is null)
            return Result.Fail<Unit>(ErrorsCodes.UserNotFound);

        var now = DateTimeOffset.UtcNow;

        var verification = await unitOfWork.GetEntityRepository<ContactVerification>().DbSet
            .Where(v =>
                v.UserId == request.UserId &&
                v.Type == ContactVerificationType.Email &&
                v.Destination == request.Email &&
                !v.UsedAt.HasValue &&
                v.ExpiresAt >= now)
            .OrderByDescending(v => v.CreatedDate)
            .FirstOrDefaultAsync(cancellationToken);

        if (verification is null || !string.Equals(verification.Code, request.Code, StringComparison.Ordinal))
            return Result.Fail<Unit>(ErrorsCodes.InvalidVerificationCode);

        verification.UsedAt = now;

        user.Email = request.Email;
        user.NormalizedEmail = userManager.NormalizeEmail(request.Email);
        user.EmailConfirmed = true;

        await userManager.UpdateAsync(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(Unit.Value);
    }
}
