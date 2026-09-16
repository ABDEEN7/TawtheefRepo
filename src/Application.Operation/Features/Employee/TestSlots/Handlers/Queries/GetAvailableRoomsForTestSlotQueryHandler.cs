using Application.Operation.Features.Employee.TestSlots.Queries;
using Application.Operation.Features.Employee.Rooms.DTOs;
using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Exams;
using Tawtheef.Domain.Entities.Lookups;

namespace Application.Operation.Features.Employee.TestSlots.Handlers.Queries;

public sealed class GetAvailableRoomsForTestSlotQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<GetAvailableRoomsForTestSlotQuery, IResult<List<RoomDto>>>
{
    public async Task<IResult<List<RoomDto>>> Handle(
        GetAvailableRoomsForTestSlotQuery request,
        CancellationToken cancellationToken)
    {
        var isArabic = string.Equals(request.Language, "ar", StringComparison.OrdinalIgnoreCase);
        var rooms = await unitOfWork.GetEntityRepository<Room>().DbSet
            .AsNoTracking()
            .Include(room => room.Location)
            .Include(room => room.RoomType)
            .Include(room => room.Status)
            .Where(room => room.StatusId == RoomStatusIds.Active)
            .OrderBy(room => isArabic ? room.NameAr : room.NameEn ?? room.NameAr)
            .ToListAsync(cancellationToken);

        return Result.Ok(mapper.Map<List<RoomDto>>(rooms));
    }
}
