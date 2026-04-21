using Application.Operation.Features.Employee.JobManagement.JobOperations.Queries;
using MediatR;
using FluentResults;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models;
using Tawtheef.Domain.Entities.Lookups;

namespace Application.Operation.Features.Employee.JobManagement.JobOperations.Handlers.Queries;


public sealed class GetManagementsBySectorQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<GetManagementsBySectorQuery, IResult<List<DropdownOptions>>>
{

    public async Task<IResult<List<DropdownOptions>>> Handle(GetManagementsBySectorQuery request, CancellationToken cancellationToken)
    {
        var dbSet = unitOfWork.GetEntityRepository<Management>().DbSet;

        var entities = await dbSet
            .AsNoTracking()
            .Where(m => m.SectorId == request.SectorId)
            .OrderBy(x => x.DisplayOrder)
            .ToListAsync(cancellationToken);

        return Result.Ok(mapper.Map<List<DropdownOptions>>(entities));
    }
}

