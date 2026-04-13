using Application.Operation.Features.Admin.Cities.Commands;
using MediatR;
using FluentResults;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Application.Operation.Features.Admin.Cities.Handlers.Commands;

public sealed class UpdateCityStatusCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateCityStatusCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(UpdateCityStatusCommand request, CancellationToken cancellationToken)
    {
        var repository = unitOfWork.GetEntityRepository<City>();
        var result = await repository.GetByIdAsync(request.CityId, cancellationToken);

        if (result.IsFailed || result.Value == null)
            return Result.Fail<Unit>(ErrorsCodes.CityNotFound);

        var city = result.Value;
        
        city.IsActive = request.IsActive;

        await repository.UpdateAsync(city, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(Unit.Value);
    }
}
