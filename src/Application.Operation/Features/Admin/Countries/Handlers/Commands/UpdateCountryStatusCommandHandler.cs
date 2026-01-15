using Application.Operation.Features.Admin.Countries.Commands;
using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Application.Operation.Features.Admin.Countries.Handlers.Commands;

public sealed class UpdateCountryStatusCommandHandler(
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider,
    ICurrentUserService currentUserService)
    : ICommandHandler<UpdateCountryStatusCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(
        UpdateCountryStatusCommand request,
        CancellationToken cancellationToken)
    {
        var repository = unitOfWork.GetEntityRepository<Country>();
        var country = await repository.DbSet
            .FirstOrDefaultAsync(c => c.Id == request.CountryId, cancellationToken);

        if (country is null)
            return Result.Fail<Unit>(ErrorsCodes.CountryNotFound);

        var now = timeProvider.GetUtcNow();
        var hasUser = Guid.TryParse(currentUserService.UserId, out var userId);

        country.IsActive = request.IsActive;
        country.UpdatedDate = now;
        country.UpdatedById = hasUser ? userId : country.UpdatedById;

        await repository.UpdateAsync(country);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(Unit.Value);
    }
}
