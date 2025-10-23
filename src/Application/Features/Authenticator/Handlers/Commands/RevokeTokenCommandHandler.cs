using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Features.Authenticator.Commands;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities;

namespace Tawtheef.Application.Features.Authenticator.Handlers.Commands;

public class RevokeTokenCommandHandler(
    IUnitOfWork uow,
    ITokenService tokenService)
    : IRequestHandler<RevokeTokenCommand, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(RevokeTokenCommand request, CancellationToken cancellationToken)
    {
        var refreshToken = await uow.GetEntityRepository<RefreshToken>().DbSet
            .FirstOrDefaultAsync(x => x.Token == request.RefreshToken, CancellationToken.None);
        if (refreshToken is null)
            return Result.Failure<Unit>(ErrorsCodes.RefreshTokenNotFound);
        
        if (refreshToken is not { IsActive: true })
        {
            return Result.Failure<Unit>(ErrorsCodes.InactiveRefreshToken);
        }

        await tokenService.RevokeRefreshToken(refreshToken, request.IpAddress, "Revoked without replacement");
        return Result.Success(Unit.Value);
    }
}