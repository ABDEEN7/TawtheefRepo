using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Operations.Admin.Universities.DTOs;
using Tawtheef.Application.Features.Operations.Admin.Universities.Queries;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Tawtheef.Application.Features.Operations.Admin.Universities.Handlers.Queries;

public sealed class GetUniversityDetailsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<GetUniversityDetailsQuery, IResult<UniversityAdminDto>>
{
    public async Task<IResult<UniversityAdminDto>> Handle(
        GetUniversityDetailsQuery request,
        CancellationToken cancellationToken)
    {
        var university = await unitOfWork
            .GetEntityRepository<University>()
            .DbSet
            .AsNoTracking()
            .Include(u => u.City)!.ThenInclude(c => c!.Country)
            .FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);

        if (university is null)
            return Result.Fail<UniversityAdminDto>(ErrorsCodes.UniversityNotFound);

        var dto = mapper.Map<UniversityAdminDto>(university);
        return Result.Ok(dto);
    }
}
