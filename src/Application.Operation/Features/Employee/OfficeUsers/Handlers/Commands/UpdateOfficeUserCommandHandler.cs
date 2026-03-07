using Application.Operation.Features.Employee.OfficeUsers.Commands;
using MediatR;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.OfficeUsers.Handlers.Commands;

public sealed class UpdateOfficeUserCommandHandler(
    UserManager<User> userManager,
    ICurrentUserService currentUserService)
    : IRequestHandler<UpdateOfficeUserCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(
        UpdateOfficeUserCommand request,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(currentUserService.UserId, out var currentUserId))
            return Result.Fail<Unit>(ErrorsCodes.InvalidUserIdentifier);

        var officeAdmin = await userManager.Users
            .OfType<OfficeUser>()
            .FirstOrDefaultAsync(user => user.Id == currentUserId && !user.IsDeleted, cancellationToken);

        if (officeAdmin?.OfficeId is null)
            return Result.Fail<Unit>(ErrorsCodes.OfficeAdminNotFound);

        var officeUser = await userManager.Users
            .OfType<OfficeUser>()
            .FirstOrDefaultAsync(
                user => user.Id == request.UserId && user.OfficeId == officeAdmin.OfficeId && !user.IsDeleted,
                cancellationToken);

        if (officeUser is null)
            return Result.Fail<Unit>(ErrorsCodes.OfficeUserNotFound);

        var email = request.Email.Trim();
        if (string.IsNullOrWhiteSpace(email))
            return Result.Fail<Unit>(ErrorsCodes.EmailRequired);

        var nameAr = request.NameAr.Trim();
        if (string.IsNullOrWhiteSpace(nameAr))
            return Result.Fail<Unit>(ErrorsCodes.NameArRequired);

        var nameEn = request.NameEn.Trim();
        if (string.IsNullOrWhiteSpace(nameEn))
            return Result.Fail<Unit>(ErrorsCodes.NameEnRequired);

        if (officeUser.Id == currentUserId
            && !string.Equals(officeUser.Email, email, StringComparison.OrdinalIgnoreCase))
            return Result.Fail<Unit>(ErrorsCodes.OfficeAdminEmailChangeNotAllowed);

        if (!string.Equals(officeUser.Email, email, StringComparison.OrdinalIgnoreCase))
        {
            var emailExists = await userManager.Users
                .IgnoreQueryFilters()
                .AnyAsync(user => user.Email == email && user.Id != officeUser.Id, cancellationToken);

            if (emailExists)
                return Result.Fail<Unit>(ErrorsCodes.EmailAlreadyInUse);

            officeUser.Email = email;
            officeUser.UserName = email;
        }

        officeUser.FullNameAr = nameAr;
        officeUser.FullNameEn = nameEn;

        var updateResult = await userManager.UpdateAsync(officeUser);
        if (!updateResult.Succeeded)
            return Result.Fail<Unit>(ErrorsCodes.OfficeAdminCreationFailed);

        return Result.Ok(Unit.Value);
    }
}

