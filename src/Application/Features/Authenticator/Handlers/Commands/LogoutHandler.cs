using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Authenticator.Commands;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Authenticator.Handlers.Commands;

public class LogoutHandler(IUnitOfWork uow, SignInManager<User> signInManager) : IRequestHandler<LogoutCommand, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        // Find the user by ID
        var user = await uow.GetUserRepository<User>().DbSet
            .Include(nameof(User.RefreshTokens))
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
        if (user == null)
            return Result.Failure<Unit>(ErrorsCodes.UserNotFound);

        uow.GetEntityRepository<RefreshToken>().DbSet
            .RemoveRange(user.RefreshTokens.Where(rt => rt.IsActive));
        
        // Sign out the user
        await signInManager.SignOutAsync();

        // Optionally, you can clear any session or token data here if needed

        return Result.Success(Unit.Value);
    }
}