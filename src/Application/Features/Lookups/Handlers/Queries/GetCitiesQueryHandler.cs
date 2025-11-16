using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Lookups.Queries;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Application.Features.Lookups.Handlers.Queries;

public sealed class GetCitiesQueryHandler(
    IUnitOfWork unitOfWork)
    : BaseLookupQueryHandler<City, GetCitiesQuery>(unitOfWork);