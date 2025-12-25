using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Operations.Admin.Offices.DTOs;
using Tawtheef.Application.Features.Operations.Admin.Offices.Queries;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Operations.Admin.Offices.Handlers.Queries;

public sealed class GetOfficeDetailsQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper)
    : IRequestHandler<GetOfficeDetailsQuery, IResult<OfficeDetailsDto>>
{
    public async Task<IResult<OfficeDetailsDto>> Handle(GetOfficeDetailsQuery request, CancellationToken cancellationToken)
    {
        var officeRepo = unitOfWork.GetEntityRepository<Office>();

        var office = await officeRepo.DbSet
            .Include(a => a.OfficeAdmin)
            .Include(o => o.Country)
            .Include(o => o.SupportedCountries)!
                .ThenInclude(sc => sc.Country)
            .Include(o => o.OfficeUsers)
            .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);

        if (office is null)
            return Result.Fail<OfficeDetailsDto>(ErrorsCodes.OfficeNotFound);

        var details = mapper.Map<OfficeDetailsDto>(office);

        return Result.Ok(details);
    }
}
