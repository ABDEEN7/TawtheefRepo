using Application.Operation.Features.Admin.TargetEntities.DTOs;
using Application.Operation.Features.Admin.TargetEntities.Queries;
using MediatR;
using FluentResults;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;

namespace Application.Operation.Features.Admin.TargetEntities.Handlers.Queries;

public sealed class GetTargetEntityDetailsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<GetTargetEntityDetailsQuery, IResult<TargetEntityAdminDto>>
{
    public async Task<IResult<TargetEntityAdminDto>> Handle(
        GetTargetEntityDetailsQuery request,
        CancellationToken cancellationToken)
    {
        var targetEntity = await unitOfWork
            .GetEntityRepository<TargetEntity>()
            .DbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (targetEntity is null)
            return Result.Fail<TargetEntityAdminDto>(ErrorsCodes.TargetEntityNotFound);

        var dto = mapper.Map<TargetEntityAdminDto>(targetEntity);
        return Result.Ok(dto);
    }
}

