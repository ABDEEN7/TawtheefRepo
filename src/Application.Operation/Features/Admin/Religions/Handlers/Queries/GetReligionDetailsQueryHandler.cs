using Application.Operation.Features.Admin.Religions.DTOs;
using Application.Operation.Features.Admin.Religions.Queries;
using MediatR;
using FluentResults;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;

namespace Application.Operation.Features.Admin.Religions.Handlers.Queries;

public sealed class GetReligionDetailsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<GetReligionDetailsQuery, IResult<ReligionAdminDto>>
{
    public async Task<IResult<ReligionAdminDto>> Handle(
        GetReligionDetailsQuery request,
        CancellationToken cancellationToken)
    {
        var religion = await unitOfWork
            .GetEntityRepository<Religion>()
            .DbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == request.Id, cancellationToken);

        if (religion is null)
            return Result.Fail<ReligionAdminDto>(ErrorsCodes.ReligionNotFound);

        var dto = mapper.Map<ReligionAdminDto>(religion);
        return Result.Ok(dto);
    }
}

