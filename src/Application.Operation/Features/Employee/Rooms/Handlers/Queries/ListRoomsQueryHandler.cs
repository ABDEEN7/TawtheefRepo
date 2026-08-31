using Application.Operation.Features.Employee.Rooms.DTOs;
using Application.Operation.Features.Employee.Rooms.Queries;
using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Extensions;
using Tawtheef.Domain.Entities.Exams;

namespace Application.Operation.Features.Employee.Rooms.Handlers.Queries;

public sealed class ListRoomsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<ListRoomsQuery, IResult<PaginatedResult<RoomDto>>>
{
    public async Task<IResult<PaginatedResult<RoomDto>>> Handle(
        ListRoomsQuery request,
        CancellationToken cancellationToken)
    {
        var searchTerm = request.Search?.Trim();
        var rooms = await unitOfWork
            .GetEntityRepository<Room>()
            .DbSet
            .AsNoTracking()
            .Include(room => room.Location)
            .Include(room => room.RoomType)
            .Include(room => room.Status)
            .WhereIf(
                !string.IsNullOrWhiteSpace(searchTerm),
                room => EF.Functions.Like(room.NameAr, $"%{searchTerm}%") ||
                        EF.Functions.Like(room.NameEn, $"%{searchTerm}%"))
            .WhereIf(request.LocationId.HasValue, room => room.LocationId == request.LocationId)
            .WhereIf(request.RoomTypeId.HasValue, room => room.RoomTypeId == request.RoomTypeId)
            .WhereIf(request.StatusId.HasValue, room => room.StatusId == request.StatusId)
            .OrderByDescending(room => room.UpdatedDate ?? room.CreatedDate)
            .ThenBy(room => room.NameEn)
            .ToPaginatedListAsync<Room, RoomDto>(mapper, request, cancellationToken);

        return Result.Ok(rooms);
    }
}
