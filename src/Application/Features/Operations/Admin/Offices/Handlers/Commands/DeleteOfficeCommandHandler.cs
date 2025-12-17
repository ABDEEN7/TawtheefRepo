using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Operations.Admin.Offices.Commands;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Operations.Admin.Offices.Handlers.Commands;

public sealed class DeleteOfficeCommandHandler(UserManager<User> userManager,IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteOfficeCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(DeleteOfficeCommand request, CancellationToken cancellationToken)
    {
        var office = await unitOfWork.GetEntityRepository<Office>().DbSet
            .Include(o => o.SupportedCountries)
            .Include(o => o.OfficeUsers)
            .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);

        if (office is null)
            return Result.Fail<Unit>(ErrorsCodes.OfficeNotFound);

        unitOfWork.RemoveRange(office.SupportedCountries);
        if (office.OfficeUsers is { Count: > 0 })
        {
            var userIds = office.OfficeUsers
                .Select(o => o.Id)
                .ToHashSet(); 

            await userManager.Users
                .Where(u => userIds.Contains(u.Id))
                .ExecuteDeleteAsync(cancellationToken);
        }
        
        unitOfWork.Remove(office);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(Unit.Value);
    }
}
