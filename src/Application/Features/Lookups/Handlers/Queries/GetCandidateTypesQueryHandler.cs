using MapsterMapper;
using Microsoft.Extensions.Caching.Memory;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Lookups.Queries;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Application.Features.Lookups.Handlers.Queries;

public sealed class GetCandidateTypesQueryHandler(
    IUnitOfWork unitOfWork, IMapper mapper, IMemoryCache cache)
    : BaseLookupQueryHandler<CandidateType, GetCandidateTypesQuery>(unitOfWork, mapper, cache);
