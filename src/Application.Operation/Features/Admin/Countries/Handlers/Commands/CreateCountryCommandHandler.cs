using Application.Operation.Features.Admin.Countries.Commands;
using MediatR;
using FluentResults;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Application.Operation.Features.Admin.Countries.Handlers.Commands;

public sealed class CreateCountryCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CreateCountryCommand, IResult<Guid>>
{
    public async Task<IResult<Guid>> Handle(CreateCountryCommand request, CancellationToken cancellationToken)
    {
        var country = new Country
        {
            NameAr = request.NameAr,
            NameEn = request.NameEn,
            BackendName = GenerateUniqueBackendName(request.NameEn),
            Code = request.Code,
            ISOCode = request.ISOCode,
            CodeAlpha = request.CodeAlpha,
            IsActive = request.IsActive
        };

        await unitOfWork.GetEntityRepository<Country>().AddAsync(country, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(country.Id);
    }
    
    private string GenerateUniqueBackendName(string nameEn)
    {
        return $"{nameEn.Replace(" ","_")}_{Guid.NewGuid()}";
    }
}
