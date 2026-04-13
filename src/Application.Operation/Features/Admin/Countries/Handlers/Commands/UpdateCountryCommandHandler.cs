using Application.Operation.Features.Admin.Countries.Commands;
using MediatR;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Application.Operation.Features.Admin.Countries.Handlers.Commands;

public sealed class UpdateCountryCommandHandler(
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider,
    ICurrentUserService currentUserService)
    : IRequestHandler<UpdateCountryCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(
        UpdateCountryCommand request,
        CancellationToken cancellationToken)
    {
        var repository = unitOfWork.GetEntityRepository<Country>();
        var country = await repository.DbSet
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (country is null)
            return Result.Fail<Unit>(ErrorsCodes.CountryNotFound);

        var now = timeProvider.GetUtcNow().UtcDateTime;
        var hasUser = Guid.TryParse(currentUserService.UserId, out var userId);

        country.NameAr = request.NameAr;
        country.NameEn = request.NameEn;
        country.Code = request.Code;
        country.ISOCode = request.ISOCode;
        country.CodeAlpha = request.CodeAlpha;
        country.IsActive = request.IsActive;
        country.UpdatedDate = now;
        country.UpdatedById = hasUser ? userId : country.UpdatedById;

        await repository.UpdateAsync(country, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(Unit.Value);
    }
}
