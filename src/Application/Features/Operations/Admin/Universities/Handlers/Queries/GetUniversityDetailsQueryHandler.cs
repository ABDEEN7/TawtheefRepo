using Cortex.Mediator.Queries;
using FluentResults;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Common.Mappers;
using Tawtheef.Application.Features.Operations.Admin.Universities.DTOs;
using Tawtheef.Application.Features.Operations.Admin.Universities.Queries;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Tawtheef.Application.Features.Operations.Admin.Universities.Handlers.Queries;

public sealed class GetUniversityDetailsQueryHandler(IUnitOfWork unitOfWork, IMediaUrlResolver media, IMapper mapper)
    : IQueryHandler<GetUniversityDetailsQuery, IResult<UniversityAdminDto>>
{
    public async Task<IResult<UniversityAdminDto>> Handle(
        GetUniversityDetailsQuery request,
        CancellationToken cancellationToken)
    {
        var university = await unitOfWork
            .GetEntityRepository<University>()
            .DbSet
            .AsNoTracking()
            .Include(u => u.City!.Country)
            .Include(u => u.LogoAr)
            .Include(u => u.LogoEn)
            .FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);

        if (university is null)
            return Result.Fail<UniversityAdminDto>(ErrorsCodes.UniversityNotFound);
        
        using var scope = new MapContextScope();
        scope.Context.Parameters[ResourceMapper.MediaKey] = media;
        var dto = mapper.Map<UniversityAdminDto>(university);
        return Result.Ok(dto);
    }
}
