using Application.Operation.Features.Employee.CandidateUsers.Commands;
using Application.Operation.Features.Employee.CandidateUsers.Services;
using MediatR;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.CandidateUsers.Handlers.Commands;

internal sealed class UpdateCandidateUserBlockStatusCommandHandler(
    UserManager<User> userManager,
    ITokenService tokenService,
    CandidateUsersAccessScope accessScope)
    : IRequestHandler<UpdateCandidateUserBlockStatusCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(
        UpdateCandidateUserBlockStatusCommand request,
        CancellationToken cancellationToken)
    {
        var populationResult = await accessScope.GetPopulationAsync(cancellationToken);
        if (populationResult.IsFailed)
            return Result.Fail<Unit>(populationResult.Errors);

        var authorizedUserIds = populationResult.Value.Users.Select(candidate => candidate.Id);
        var user = await userManager.Users
            .OfType<ApplicantUser>()
            .FirstOrDefaultAsync(
                candidate => candidate.Id == request.UserId && authorizedUserIds.Contains(candidate.Id),
                cancellationToken);

        if (user is null)
            return Result.Fail<Unit>(ErrorsCodes.UserNotFound);

        user.IsBlocked = request.IsBlocked;

        var updateResult = await userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
            return Result.Fail<Unit>(string.Join(", ", updateResult.Errors.Select(e => e.Description)));

        if (user.IsBlocked)
        {
            await tokenService.RevokeAllAsync(user.Id, cancellationToken);
        }
        else
        {
            await tokenService.ClearUserCacheAsync(user.Id, cancellationToken);
        }

        return Result.Ok(Unit.Value);
    }
}

