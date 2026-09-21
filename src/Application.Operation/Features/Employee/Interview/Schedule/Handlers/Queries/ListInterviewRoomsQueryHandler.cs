using Application.Operation.Features.Employee.Interview.Schedule.DTOs;
using Application.Operation.Features.Employee.Interview.Schedule.Queries;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Extensions;
using Tawtheef.Domain.Entities.Exams;
using Tawtheef.Domain.Entities.Lookups;

namespace Application.Operation.Features.Employee.Interview.Schedule.Handlers.Queries;

public sealed class ListInterviewRoomsQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<ListInterviewRoomsQuery, IResult<List<RoomOptionDto>>>
{
    // The dropdown is search-as-you-type, so a capped, name-ordered result replaces paging.
    private const int MaxResults = 30;

    public async Task<IResult<List<RoomOptionDto>>> Handle(ListInterviewRoomsQuery request, CancellationToken cancellationToken)
    {
        var search = request.Search?.Trim();
        var hasSearch = !string.IsNullOrEmpty(search);

        var rooms = await unitOfWork.GetEntityRepository<Room>().DbSet
            .AsNoTracking()
            .Include(r => r.Location)
            .Where(r => r.RoomTypeId == RoomTypeIds.InterviewRoom && r.StatusId == RoomStatusIds.Active)
            .WhereIf(hasSearch, r => r.NameAr.Contains(search!) || (r.NameEn != null && r.NameEn.Contains(search!)))
            .OrderBy(r => r.NameAr)
            .Take(MaxResults)
            .Select(r => new RoomOptionDto(
                r.Id, r.NameAr, r.NameEn, r.Location!.NameAr, r.Location.NameEn, r.Capacity))
            .ToListAsync(cancellationToken);

        return Result.Ok(rooms);
    }
}
