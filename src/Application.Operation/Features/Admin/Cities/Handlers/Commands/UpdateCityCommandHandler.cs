using Application.Operation.Features.Admin.Cities.Commands;
using MediatR;
using FluentResults;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Application.Operation.Features.Admin.Cities.Handlers.Commands;

public sealed class UpdateCityCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateCityCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(UpdateCityCommand request, CancellationToken cancellationToken)
    {
        var repository = unitOfWork.GetEntityRepository<City>();
        var result = await repository.GetByIdAsync(request.Id, cancellationToken);

        if (result.IsFailed || result.Value == null)
            return Result.Fail<Unit>("City not found");

        var city = result.Value;
        
        city.CountryId = request.CountryId;
        city.NameAr = request.NameAr;
        city.NameEn = request.NameEn;
        city.Code = request.Code;
        city.IsActive = request.IsActive;

        await repository.UpdateAsync(city, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(Unit.Value);
    }
}
