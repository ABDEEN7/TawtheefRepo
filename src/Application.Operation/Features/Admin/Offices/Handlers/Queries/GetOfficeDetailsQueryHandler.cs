using Application.Operation.Features.Admin.Offices.DTOs;
using Application.Operation.Features.Admin.Offices.Queries;
using Cortex.Mediator.Queries;
using FluentResults;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Application.Operation.Features.Admin.Offices.Handlers.Queries;

public sealed class GetOfficeDetailsQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper)
    : IQueryHandler<GetOfficeDetailsQuery, IResult<OfficeDetailsDto>>
{
    public async Task<IResult<OfficeDetailsDto>> Handle(GetOfficeDetailsQuery request, CancellationToken cancellationToken)
    {
        var officeRepo = unitOfWork.GetEntityRepository<Office>();

        var office = await officeRepo.DbSet
            .Include(a => a.OfficeAdmin)
            .Include(o => o.Country)
            .Include(o => o.SupportedCountries)
                .ThenInclude(sc => sc.Country)
            .Include(o => o.OfficeUsers)
            .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);

        if (office is null)
            return Result.Fail<OfficeDetailsDto>(ErrorsCodes.OfficeNotFound);

        var details = mapper.Map<OfficeDetailsDto>(office);

        return Result.Ok(details);
    }
}
