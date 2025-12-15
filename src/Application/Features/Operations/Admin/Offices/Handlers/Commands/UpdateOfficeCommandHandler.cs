using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Operations.Admin.Offices.Commands;
using Tawtheef.Application.Features.Operations.Admin.Offices.DTOs;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Tawtheef.Application.Features.Operations.Admin.Offices.Handlers.Commands;

public sealed class UpdateOfficeCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<UpdateOfficeCommand, IResult<OfficeDto>>
{
    public async Task<IResult<OfficeDto>> Handle(UpdateOfficeCommand request, CancellationToken cancellationToken)
    {
        var officeRepo = unitOfWork.GetEntityRepository<Office>();
        var countryRepo = unitOfWork.GetEntityRepository<Country>();

        var office = await officeRepo.DbSet
            .Include(o => o.SupportedCountries)
            .Include(o => o.Country)
            .Include(o => o.OfficeUsers)
            .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);

        if (office is null)
            return Result.Fail<OfficeDto>(ErrorsCodes.OfficeNotFound);

        var requestedIds = request.SupportedCountryIds.Distinct().ToHashSet();

        var supportedCountries = await countryRepo.DbSet
            .AsNoTracking()
            .Where(c => requestedIds.Contains(c.Id))
            .ToListAsync(cancellationToken);

        if (requestedIds.Count != supportedCountries.Count)
            return Result.Fail<OfficeDto>(ErrorsCodes.OfficeSupportedCountryInvalid);

        office.NameAr = request.NameAr;
        office.NameEn = request.NameEn;
        var existingByCountry = office.SupportedCountries.ToDictionary(sc => sc.CountryId, sc => sc);

        var toRemove = office.SupportedCountries.Where(sc => !requestedIds.Contains(sc.CountryId)).ToList();
        unitOfWork.RemoveRange(toRemove);

        foreach (var country in supportedCountries)
        {
            if (existingByCountry.TryGetValue(country.Id, out var existing))
            {
                existing.NameAr = country.NameAr;
                existing.NameEn = country.NameEn;
                existing.DescriptionAr = country.DescriptionAr;
                existing.DescriptionEn = country.DescriptionEn;
            }
            else
            {
                office.SupportedCountries.Add(new OfficeSupportedCountry
                {
                    Id = Guid.NewGuid(),
                    BackendName = $"{office.BackendName}_SUP_{country.CodeAlpha}",
                    NameAr = country.NameAr,
                    NameEn = country.NameEn,
                    DescriptionAr = country.DescriptionAr,
                    DescriptionEn = country.DescriptionEn,
                    CountryId = country.Id,
                    DisplayOrder = office.SupportedCountries.Count + 1
                });
            }
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(mapper.Map<OfficeDto>(office));
    }
}
