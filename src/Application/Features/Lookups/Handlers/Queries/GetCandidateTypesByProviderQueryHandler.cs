using Cortex.Mediator.Queries;
using FluentResults;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models;
using Tawtheef.Application.Features.Lookups.Queries;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Lookups.Handlers.Queries;

public sealed class GetCandidateTypesByProviderQueryHandler(
    IUnitOfWork unitOfWork,
    UserManager<User> userManager,
    IMapper mapper)
    : IQueryHandler<GetCandidateTypesByProviderQuery, IResult<List<DropdownOptions>>>
{
    public async Task<IResult<List<DropdownOptions>>> Handle(
        GetCandidateTypesByProviderQuery request,
        CancellationToken ct)
    {
        var candidateTypes = await GetProviderCandidateTypesAsync(request.Provider, ct);
        
        var user = await userManager.Users.OfType<ApplicantUser>().AsNoTracking()
            .SingleAsync(x => x.Id == request.UserId, ct);
        if (!user.IsUserKawader)
            return Result.Ok(mapper.Map<List<DropdownOptions>>(candidateTypes));

        var qatariCandidateType = await GetQatariCandidateTypeAsync(ct);
        candidateTypes.AddRange(qatariCandidateType);

        return Result.Ok(mapper.Map<List<DropdownOptions>>(candidateTypes));
    }

    private async Task<List<CandidateType>> GetQatariCandidateTypeAsync(CancellationToken ct)
    {
        var qatari = await unitOfWork.GetEntityRepository<CandidateType>()
            .DbSet
            .AsNoTracking()
            .Where(x => x.Id == CandidateTypeIds.Qatari)
            .SingleOrDefaultAsync(ct);

        return qatari is null ? [] : [qatari];
    }

    private async Task<List<CandidateType>> GetProviderCandidateTypesAsync(string provider, CancellationToken ct)
    {
        var normalizedProvider = Normalize(provider);

        // Distinct avoids duplicates if provider mappings join to the same CandidateType multiple times.
        return await unitOfWork.GetEntityRepository<ProviderLogin>()
            .DbSet
            .AsNoTracking()
            .Where(x => x.IsActive)
            .Where(x => x.BackendName.ToLower() == normalizedProvider)
            .SelectMany(x => x.CandidateTypeProviderLogins)
            .Where(x => x.CandidateType != null)
            .Select(x => x.CandidateType!)
            .Distinct()
            .ToListAsync(ct);
    }

    private static string Normalize(string value) => value .Trim().ToLowerInvariant();
}
