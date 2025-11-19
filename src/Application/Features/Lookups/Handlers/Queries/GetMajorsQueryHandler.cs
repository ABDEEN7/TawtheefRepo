using MapsterMapper;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Lookups.Queries;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Tawtheef.Application.Features.Lookups.Handlers.Queries;

public sealed class GetMajorsQueryHandler(
    IUnitOfWork unitOfWork, IMapper mapper)
    : BaseLookupQueryHandler<Major, GetMajorsQuery>(unitOfWork, mapper);
