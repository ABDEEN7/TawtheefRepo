using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Authenticator.Commands;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Authenticator.Handlers.Commands.VerificationHandler;

public class RequestUpdatePhoneCommandHandler(
    IUnitOfWork unitOfWork,
    UserManager<User> userManager)
    : ICommandHandler<RequestUpdatePhoneCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(RequestUpdatePhoneCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
        if (user is null)
            return Result.Fail<Unit>(ErrorsCodes.UserNotFound);

        if (request.PhoneE164.StartsWith("+974"))
            return Result.Fail<Unit>(ErrorsCodes.ShouldVerifiyQatarPhoneNumberBeforeAssignIt);
        
        user.PhoneNumber = request.PhoneE164;
        user.PhoneNumberConfirmed = true;

        await userManager.UpdateAsync(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(Unit.Value);
    }
}
