using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models;
using Tawtheef.Application.Features.Lookups.Queries;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Application.Features.Lookups.Handlers.Queries;

public sealed class GetCandidateTypesByProviderQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<GetCandidateTypesByProviderQuery, IResult<List<DropdownOptions>>>
{
    public async Task<IResult<List<DropdownOptions>>> Handle(
        GetCandidateTypesByProviderQuery request,
        CancellationToken cancellationToken)
    {
        var candidateTypes = await unitOfWork.GetEntityRepository<ProviderLogin>()
            .DbSet
            .Where(pl => pl.BackendName.ToLower() == request.Provider.ToLower())
            .SelectMany(cl=> cl.CandidateTypeProviderLogins)
            .Select(ct => ct.CandidateType!)
            .ToListAsync(cancellationToken);
        
        return Result.Ok(mapper.Map<List<DropdownOptions>>(candidateTypes));
    }
}
