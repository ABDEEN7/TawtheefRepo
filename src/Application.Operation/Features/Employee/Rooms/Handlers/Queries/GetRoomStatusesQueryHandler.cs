using Application.Operation.Features.Employee.Rooms.Queries;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Lookups.Handlers.Queries;
using Tawtheef.Domain.Configurations.Settings;
using Tawtheef.Domain.Entities.Lookups;

namespace Application.Operation.Features.Employee.Rooms.Handlers.Queries;

public sealed class GetRoomStatusesQueryHandler(
    IUnitOfWork unitOfWork,
    IMemoryCache cache,
    IOptions<AppConfigSettings> appConfig)
    : BaseLookupQueryHandler<RoomStatus, GetRoomStatusesQuery>(unitOfWork, cache, appConfig);
