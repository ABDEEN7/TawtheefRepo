using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models;
using Tawtheef.Application.Extensions;
using Tawtheef.Application.Features.Lookups.Queries;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Tawtheef.Application.Features.Lookups.Handlers.Queries;

public sealed class GetSubMajorsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) 
    : IRequestHandler<GetSubMajorsQuery, IResult<List<DropdownOptions>>>
{
    public async Task<IResult<List<DropdownOptions>>> Handle(GetSubMajorsQuery request, CancellationToken cancellationToken)
    {
        var dbSet = unitOfWork.GetEntityRepository<Major>().DbSet;

        var entities = await dbSet
            .AsNoTracking()
            .Where(s => s.IsActive)
            .Where(x => x.ParentId == request.ParentId)
            .WhereIf(!string.IsNullOrEmpty(request.Search), 
                s => 
                    EF.Functions.Like(s.NameAr, $"%{request.Search}%") ||
                    EF.Functions.Like(s.NameEn, $"%{request.Search}%") ||
                    EF.Functions.Like(s.DescriptionAr ?? "", $"%{request.Search}%") ||
                    EF.Functions.Like(s.DescriptionEn ?? "", $"%{request.Search}%"))
            .OrderBy(x => x.DisplayOrder)
            .ToListAsync(cancellationToken);

        return Result.Ok(mapper.Map<List<DropdownOptions>>(entities));
    }
}
