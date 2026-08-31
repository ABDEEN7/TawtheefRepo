using Application.Operation.Features.Employee.Rooms.DTOs;
using Application.Operation.Features.Employee.Rooms.Queries;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Exams;

namespace Application.Operation.Features.Employee.Rooms.Handlers.Queries;

public sealed class GetRoomLocationsQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetRoomLocationsQuery, IResult<List<RoomLocationDto>>>
{
    public async Task<IResult<List<RoomLocationDto>>> Handle(
        GetRoomLocationsQuery request,
        CancellationToken cancellationToken)
    {
        var locations = await unitOfWork.GetEntityRepository<Location>().DbSet
            .AsNoTracking()
            .OrderBy(x => x.NameEn ?? x.NameAr)
            .Select(x => new RoomLocationDto
            {
                Id = x.Id,
                NameAr = x.NameAr,
                NameEn = x.NameEn,
                LocationLink = x.LocationLink
            })
            .ToListAsync(cancellationToken);

        return Result.Ok(locations);
    }
}
