using Application.Recruitment.Features.Authenticator.Commands;
using Application.Recruitment.Features.Profile.Policies;
using MediatR;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Users;

namespace Application.Recruitment.Features.Authenticator.Handlers.Commands.VerificationHandler;

public class RequestUpdatePhoneCommandHandler(
    IUnitOfWork unitOfWork,
    UserManager<User> userManager)
    : IRequestHandler<RequestUpdatePhoneCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(RequestUpdatePhoneCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
        if (user is null)
            return Result.Fail<Unit>(ErrorsCodes.UserNotFound);

        if (await IsLockedIdentityProviderAsync(user))
            return Result.Fail<Unit>(ErrorsCodes.UnauthorizedAction);

        if (request.PhoneE164.StartsWith("+974"))
            return Result.Fail<Unit>(ErrorsCodes.ShouldVerifyQatarPhoneNumberBeforeAssignIt);
        
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

