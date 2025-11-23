using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models;
using Tawtheef.Application.Features.Lookups.Queries;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Application.Features.Lookups.Handlers.Queries;

public sealed class GetGendersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) 
    : IRequestHandler<GetGendersQuery, IResult<List<DropdownOptions>>>
{
    
    public async Task<IResult<List<DropdownOptions>>> Handle(GetGendersQuery request, CancellationToken cancellationToken)
    {
        var dbSet = unitOfWork.GetEntityRepository<Gender>().DbSet;

        var entities = await dbSet
            .AsNoTracking()
            .Skip(1)
            .OrderBy(x => x.DisplayOrder)
            .ToListAsync(cancellationToken);

        return Result.Ok(mapper.Map<List<DropdownOptions>>(entities));
    }
}
