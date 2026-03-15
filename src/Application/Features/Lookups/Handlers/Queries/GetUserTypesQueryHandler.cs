using Tawtheef.Domain.Configurations.Settings;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Memory;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Lookups.Queries;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Application.Features.Lookups.Handlers.Queries;

public sealed class GetUserTypesQueryHandler(
    IUnitOfWork unitOfWork, IMemoryCache cache, IOptions<AppConfigSettings> appConfig)
    : BaseLookupQueryHandler<UserType, GetUserTypesQuery>(unitOfWork, cache, appConfig);
