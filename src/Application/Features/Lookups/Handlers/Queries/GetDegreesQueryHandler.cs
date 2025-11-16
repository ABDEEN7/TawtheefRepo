using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Lookups.Queries;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Application.Features.Lookups.Handlers.Queries;

public sealed class GetDegreesQueryHandler(
    IUnitOfWork unitOfWork)
    : BaseLookupQueryHandler<Degree, GetDegreesQuery>(unitOfWork);