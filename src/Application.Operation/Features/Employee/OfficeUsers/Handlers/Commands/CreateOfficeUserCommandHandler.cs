using Application.Operation.Features.Employee.OfficeUsers.Commands;
using Cortex.Mediator.Commands;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.OfficeUsers.Handlers.Commands;

public sealed class CreateOfficeUserCommandHandler(
    UserManager<User> userManager,
    ICurrentUserService currentUserService)
    : ICommandHandler<CreateOfficeUserCommand, IResult<Guid>>
{
    public async Task<IResult<Guid>> Handle(
        CreateOfficeUserCommand request,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(currentUserService.UserId, out var currentUserId))
            return Result.Fail<Guid>(ErrorsCodes.InvalidUserIdentifier);

        var officeAdmin = await userManager.Users
            .OfType<OfficeUser>()
            .FirstOrDefaultAsync(user => user.Id == currentUserId && !user.IsDeleted, cancellationToken);

        if (officeAdmin?.OfficeId is null)
            return Result.Fail<Guid>(ErrorsCodes.OfficeAdminNotFound);

        var email = request.Email?.Trim();
        if (string.IsNullOrWhiteSpace(email))
            return Result.Fail<Guid>(ErrorsCodes.EmailRequired);

        var emailExists = await userManager.Users
            .IgnoreQueryFilters()
            .AnyAsync(user => user.Email == email, cancellationToken);

        if (emailExists)
            return Result.Fail<Guid>(ErrorsCodes.EmailAlreadyInUse);

        var nameAr = request.NameAr?.Trim() ?? string.Empty;
        var nameEn = request.NameEn?.Trim() ?? string.Empty;

        var registerResult = OfficeUser.Register(email, nameAr, nameEn);
        if (registerResult.IsFailed)
            return Result.Fail<Guid>(registerResult.Errors);

        if (registerResult.Value is not OfficeUser officeUser)
            return Result.Fail<Guid>(ErrorsCodes.OfficeUserNotFound);

        officeUser.OfficeId = officeAdmin.OfficeId;

        var createResult = await userManager.CreateAsync(officeUser);
        if (!createResult.Succeeded)
            return Result.Fail<Guid>(ErrorsCodes.OfficeAdminCreationFailed);

        var roleResult = await userManager.AddToRoleAsync(officeUser, nameof(SystemRoleIds.OfficeUser));
        if (!roleResult.Succeeded)
            return Result.Fail<Guid>(ErrorsCodes.OfficeRoleAssignmentFailed);

        return Result.Ok(officeUser.Id);
    }
}
