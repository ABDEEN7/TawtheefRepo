using Cortex.Mediator.Queries;
using FluentResults;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models;
using Tawtheef.Application.Features.Lookups.Queries;
using Tawtheef.Domain.Entities.Kawader;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Lookups.Handlers.Queries;

public sealed class GetCandidateTypesByProviderQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper)
    : IQueryHandler<GetCandidateTypesByProviderQuery, IResult<List<DropdownOptions>>>
{
    public async Task<IResult<List<DropdownOptions>>> Handle(
        GetCandidateTypesByProviderQuery request,
        CancellationToken ct)
    {
        var userQid = await GetUserQidAsync(request.UserId, ct);

        List<CandidateType> candidateTypes;
        // If user doesn't have a QID, treat as non-Kawader and return provider mapping (or empty list).
        if (string.IsNullOrWhiteSpace(userQid))
        {
            candidateTypes = await GetProviderCandidateTypesAsync(request.Provider, ct);
        }
        else
        {
            var isKawaderUser = await IsKawaderUserAsync(userQid, ct);
            candidateTypes = isKawaderUser
                ? await GetQatariCandidateTypeAsync(ct)
                : await GetProviderCandidateTypesAsync(request.Provider, ct);
        }

        return Result.Ok(mapper.Map<List<DropdownOptions>>(candidateTypes));
    }

    private async Task<string?> GetUserQidAsync(Guid userId, CancellationToken ct)
    {
        return await unitOfWork.GetEntityRepository<UserProfile>()
            .DbSet
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .Select(x => x.NationalNumber)
            .FirstOrDefaultAsync(ct);
    }

    private async Task<bool> IsKawaderUserAsync(string qid, CancellationToken ct)
    {
        return await unitOfWork.GetEntityRepository<KawaderQid>()
            .DbSet
            .AsNoTracking()
            .AnyAsync(x => x.Qid == qid, ct);
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
