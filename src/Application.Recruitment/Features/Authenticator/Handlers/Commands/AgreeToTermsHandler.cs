using Application.Recruitment.Features.Authenticator.Commands;
using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Users;

namespace Application.Recruitment.Features.Authenticator.Handlers.Commands;

public sealed class AgreeToTermsHandler(IUnitOfWork uow, UserManager<User> userManager) 
    : ICommandHandler<AgreeToTermsCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(AgreeToTermsCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user is null)
            return Result.Fail<Unit>(ErrorsCodes.UserNotFound);

        if (user.AgreedToTerms)
            return Result.Ok(Unit.Value);

        user.AgreedToTerms = true;
        await uow.SaveChangesAsync(cancellationToken);

        return Result.Ok(Unit.Value);
    }
}

