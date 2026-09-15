using Application.Operation.Features.Employee.TestSlots.Queries;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models;
using Tawtheef.Domain.Entities.Exams;

namespace Application.Operation.Features.Employee.TestSlots.Handlers.Queries;

public sealed class GetAvailableRoomsForTestSlotQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetAvailableRoomsForTestSlotQuery, IResult<List<DropdownOptions>>>
{
    public async Task<IResult<List<DropdownOptions>>> Handle(
        GetAvailableRoomsForTestSlotQuery request,
        CancellationToken cancellationToken)
    {
        var isArabic = string.Equals(request.Language, "ar", StringComparison.OrdinalIgnoreCase);
        var availableRooms = await unitOfWork.GetEntityRepository<Room>().DbSet
            .AsNoTracking()
            .OrderBy(room => isArabic ? room.NameAr : room.NameEn ?? room.NameAr)
            .Select(room => new DropdownOptions
            {
                Id = room.Id,
                BackendName = room.NameEn ?? room.NameAr,
                Name = isArabic ? room.NameAr : room.NameEn ?? room.NameAr
            })
            .ToListAsync(cancellationToken);

        return Result.Ok(availableRooms);
    }
}
