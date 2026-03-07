using Microsoft.Extensions.Caching.Memory;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Lookups.Queries;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Tawtheef.Application.Features.Lookups.Handlers.Queries;

public sealed class GetSkillsQueryHandler(
    IUnitOfWork unitOfWork,  IMemoryCache cache)
    : BaseLookupQueryHandler<Skill, GetSkillsQuery>(unitOfWork, cache);
