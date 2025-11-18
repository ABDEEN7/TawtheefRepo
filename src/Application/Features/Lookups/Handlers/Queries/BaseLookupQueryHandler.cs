using CSharpFunctionalExtensions;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models;
using Tawtheef.Domain.Common;

namespace Tawtheef.Application.Features.Lookups.Handlers.Queries;

public abstract class BaseLookupQueryHandler<TLookup, TRequest>(IUnitOfWork unitOfWork)
    : IRequestHandler<TRequest, Result<List<DropdownOptions>>>
    where TLookup : LookupBase
    where TRequest : IRequest<Result<List<DropdownOptions>>>
{
    public async Task<Result<List<DropdownOptions>>> Handle(TRequest request, CancellationToken cancellationToken)
    {
        var dbSet = unitOfWork.GetEntityRepository<TLookup>().DbSet;

        var entities = await dbSet
            .AsNoTracking()
            .OrderBy(x => x.DisplayOrder)
            .ToListAsync(cancellationToken);

        return Result.Success(entities.Adapt<List<DropdownOptions>>());
    }
}
