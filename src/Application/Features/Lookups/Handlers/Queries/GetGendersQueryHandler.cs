using MapsterMapper;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Lookups.Queries;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Application.Features.Lookups.Handlers.Queries;

public sealed class GetGendersQueryHandler(
    IUnitOfWork unitOfWork, IMapper mapper)
    : BaseLookupQueryHandler<Gender, GetGendersQuery>(unitOfWork, mapper);
