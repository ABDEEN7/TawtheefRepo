using Application.Operation.Features.Employee.CandidateUsers.Commands;
using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.CandidateUsers.Handlers.Commands;

public sealed class UpdateCandidateUserBlockStatusCommandHandler(UserManager<User> userManager)
    : ICommandHandler<UpdateCandidateUserBlockStatusCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(
        UpdateCandidateUserBlockStatusCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.Users
            .OfType<ApplicantUser>()
            .FirstOrDefaultAsync(u => u.Id == request.UserId && !u.IsDeleted, cancellationToken);

        if (user is null)
            return Result.Fail<Unit>(ErrorsCodes.UserNotFound);

        user.IsBlocked = request.IsBlocked;

        var updateResult = await userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
            return Result.Fail<Unit>(string.Join(", ", updateResult.Errors.Select(e => e.Description)));

        return Result.Ok(Unit.Value);
    }
}
