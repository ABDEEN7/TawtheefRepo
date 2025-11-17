using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Lookups.Queries;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Tawtheef.Application.Features.Lookups.Handlers.Queries;

public sealed class GetUniversitiesQueryHandler(
    IUnitOfWork unitOfWork)
    : BaseLookupQueryHandler<University, GetUniversitiesQuery>(unitOfWork);