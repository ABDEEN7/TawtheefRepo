using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Lookups.Queries;
using Tawtheef.Domain.Configurations.Settings;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Application.Features.Lookups.Handlers.Queries;

public sealed class GetDepartmentsQueryHandler(
    IUnitOfWork unitOfWork, IMemoryCache cache, IOptions<AppConfigSettings> appConfig)
    : BaseLookupQueryHandler<Department, GetDepartmentsQuery>(unitOfWork, cache, appConfig);
