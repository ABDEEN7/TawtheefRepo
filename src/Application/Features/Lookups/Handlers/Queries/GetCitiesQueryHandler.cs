using Tawtheef.Domain.Configurations.Settings;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Memory;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Lookups.Queries;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Tawtheef.Application.Features.Lookups.Handlers.Queries;

public sealed class GetCitiesQueryHandler(
    IUnitOfWork unitOfWork, IMemoryCache cache, IOptions<AppConfigSettings> appConfig)
    : BaseLookupQueryHandler<City, GetCitiesQuery>(unitOfWork, cache, appConfig);
