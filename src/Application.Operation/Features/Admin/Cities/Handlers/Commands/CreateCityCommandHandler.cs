using Application.Operation.Features.Admin.Cities.Commands;
using MediatR;
using FluentResults;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Application.Operation.Features.Admin.Cities.Handlers.Commands;

public sealed class CreateCityCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CreateCityCommand, IResult<Guid>>
{
    public async Task<IResult<Guid>> Handle(CreateCityCommand request, CancellationToken cancellationToken)
    {
        var city = new City
        {
            CountryId = request.CountryId,
            NameAr = request.NameAr,
            NameEn = request.NameEn,
            BackendName = GenerateUniqueBackendName(request.NameEn),
            Code = request.Code,
            IsActive = request.IsActive
        };

        await unitOfWork.GetEntityRepository<City>().AddAsync(city, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(city.Id);
    }
    
    private string GenerateUniqueBackendName(string nameEn)
    {
        return $"{nameEn.Replace(" ","_")}_{Guid.NewGuid()}";
    }
}
