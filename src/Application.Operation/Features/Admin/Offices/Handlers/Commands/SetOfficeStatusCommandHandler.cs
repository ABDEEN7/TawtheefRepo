using Application.Operation.Features.Admin.Offices.Commands;
using MediatR;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Application.Operation.Features.Admin.Offices.Handlers.Commands;

public sealed class SetOfficeStatusCommandHandler(
    IUnitOfWork unitOfWork,
    ITokenService tokenService)
    : IRequestHandler<SetOfficeStatusCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(
        SetOfficeStatusCommand request,
        CancellationToken cancellationToken)
    {
        var officeRepo = unitOfWork.GetEntityRepository<Office>();

        var office = await officeRepo.DbSet
            .Include(o => o.OfficeUsers)
            .FirstOrDefaultAsync(o => o.Id == request.OfficeId, cancellationToken);

        if (office is null)
            return Result.Fail<Unit>(ErrorsCodes.OfficeNotFound);

        office.IsActive = request.IsActive;

        var userIdsToRevoke = new List<Guid>();
        if (!request.IsActive && office.OfficeUsers is { Count: > 0 })
        {
            foreach (var user in office.OfficeUsers.Where(user => !user.IsDeleted))
            {
                user.IsBlocked = true;
                userIdsToRevoke.Add(user.Id);
            }
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        foreach (var userId in userIdsToRevoke)
            await tokenService.RevokeAllAsync(userId, cancellationToken);

        return Result.Ok(Unit.Value);
    }
}

