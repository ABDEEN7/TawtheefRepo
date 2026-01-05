using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Authenticator.Commands;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Auth;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Authenticator.Handlers.Commands.VerificationHandler;

public class ConfirmPhoneVerificationCommandHandler(
    IUnitOfWork unitOfWork,
    UserManager<User> userManager)
    : ICommandHandler<ConfirmPhoneVerificationCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(ConfirmPhoneVerificationCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
        if (user is null)
            return Result.Fail<Unit>(ErrorsCodes.UserNotFound);

        var now = DateTimeOffset.UtcNow;

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
}
