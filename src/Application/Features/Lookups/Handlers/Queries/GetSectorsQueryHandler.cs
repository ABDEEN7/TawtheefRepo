using Microsoft.Extensions.Caching.Memory;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Lookups.Queries;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Application.Features.Lookups.Handlers.Queries;

public sealed class GetSectorsQueryHandler(
    IUnitOfWork unitOfWork,  IMemoryCache cache)
    : BaseLookupQueryHandler<Sector, GetSectorsQuery>(unitOfWork, cache);
