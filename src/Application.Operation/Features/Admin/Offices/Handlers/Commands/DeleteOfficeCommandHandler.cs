using Application.Operation.Features.Admin.Offices.Commands;
using MediatR;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Admin.Offices.Handlers.Commands;

public sealed class DeleteOfficeCommandHandler(
    UserManager<User> userManager,
    IUnitOfWork unitOfWork,
    ITokenService tokenService,
    TimeProvider time)
    : IRequestHandler<DeleteOfficeCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(
        DeleteOfficeCommand request,
        CancellationToken cancellationToken)
    {
        var officeRepo = unitOfWork.GetEntityRepository<Office>();

        var office = await officeRepo.DbSet
            .Include(o => o.SupportedCountries)
            .Include(o => o.OfficeUsers)
            .FirstOrDefaultAsync(o => o.Id == request.OfficeId, cancellationToken);

        if (office is null)
            return Result.Fail<Unit>(ErrorsCodes.OfficeNotFound);
        
        unitOfWork.RemoveRange(office.SupportedCountries);
        if (office.OfficeUsers is { Count: > 0 })
        {
            var userIds = office.OfficeUsers
                .Select(u => u.Id)
                .ToList();

            await userManager.Users
                .Where(u => userIds.Contains(u.Id)).ExecuteUpdateAsync(
                    setters => setters
                        .SetProperty(u => u.IsDeleted, true)
                        .SetProperty(u => u.Email, u => "deleted_" + Guid.NewGuid().ToString().Substring(0, 8) + "_" + u.Email)
                        .SetProperty(u => u.NormalizedEmail, u => "DELETED_" + Guid.NewGuid().ToString().Substring(0, 8) + "_" + u.NormalizedEmail)
                        .SetProperty(u => u.UserName, u => "deleted_" + Guid.NewGuid().ToString().Substring(0, 8) + "_" + u.UserName)
                        .SetProperty(u => u.NormalizedUserName, u => "DELETED_" + Guid.NewGuid().ToString().Substring(0, 8) + "_" + u.NormalizedUserName)
                        .SetProperty(u => u.DeletedDate, time.GetUtcNow().UtcDateTime)
                        .SetProperty(u => u.DeletedById, request.UserId), cancellationToken);

            foreach (var userId in userIds)
            {
                // Remove external logins (e.g. Google ProviderKey) so they
                // won't resolve to this deleted user on future logins
                var user = await userManager.FindByIdAsync(userId.ToString());
                if (user is not null)
                {
                    var logins = await userManager.GetLoginsAsync(user);
                    foreach (var login in logins)
                        await userManager.RemoveLoginAsync(user, login.LoginProvider, login.ProviderKey);
                }

                await tokenService.RevokeAllAsync(userId, cancellationToken);
            }
        }
        unitOfWork.Remove(office);
        
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(Unit.Value);
    }
}

