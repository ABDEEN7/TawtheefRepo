using Application.Operation.Features.Employee.JobManagement.JobOperations.Queries;
using MediatR;
using FluentResults;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models;
using Tawtheef.Domain.Entities.Lookups;

namespace Application.Operation.Features.Employee.JobManagement.JobOperations.Handlers.Queries;


public sealed class GetDepartmentsByManagementQueryHnadler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<GetDepartmentsByManagementQuery, IResult<List<DropdownOptions>>>
{

    public async Task<IResult<List<DropdownOptions>>> Handle(GetDepartmentsByManagementQuery request, CancellationToken cancellationToken)
    {
        var dbSet = unitOfWork.GetEntityRepository<Department>().DbSet;

        var entities = await dbSet
            .AsNoTracking()
            .Where(m => m.ManagementId == request.ManagementId)
            .OrderBy(x => x.DisplayOrder)
            .ToListAsync(cancellationToken);

        return Result.Ok(mapper.Map<List<DropdownOptions>>(entities));
    }
}

