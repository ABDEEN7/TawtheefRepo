using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Features.Operations.Admin.Countries.Commands;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Tawtheef.Application.Features.Operations.Admin.Countries.Handlers.Commands;

public sealed class UpdateCountryStatusCommandHandler(
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider,
    ICurrentUserService currentUserService)
    : IRequestHandler<UpdateCountryStatusCommand, IResult<Unit>>
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
