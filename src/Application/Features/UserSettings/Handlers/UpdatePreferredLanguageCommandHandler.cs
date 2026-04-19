using MediatR;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Tawtheef.Application.Features.UserSettings.Commands;
using Tawtheef.Domain.Constants;

namespace Tawtheef.Application.Features.UserSettings.Handlers;

public sealed class UpdatePreferredLanguageCommandHandler(
    UserManager<Domain.Entities.Users.User> userManager)
    : IRequestHandler<UpdatePreferredLanguageCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(UpdatePreferredLanguageCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString());
        if (user is null)
            return Result.Fail<Unit>(ErrorsCodes.UserNotFound);

        user.PreferredLanguage = request.PreferredLanguage;
        var result = await userManager.UpdateAsync(user);

        return result.Succeeded ? Result.Ok(Unit.Value) : Result.Fail<Unit>(result.Errors.Select(e => e.Description));
    }
}
