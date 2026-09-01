using Application.Operation.Features.Employee.Locations.Commands;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Exams;

namespace Application.Operation.Features.Employee.Locations.Handlers.Commands;

public sealed class DeleteLocationCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteLocationCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(
        DeleteLocationCommand request,
        CancellationToken cancellationToken)
    {
        var locationRepository = unitOfWork.GetEntityRepository<Location>();
        var location = await locationRepository.DbSet
            .FirstOrDefaultAsync(x => x.Id == request.LocationId, cancellationToken);

        if (location is null)
            return Result.Fail<Unit>(ErrorsCodes.LocationNotFound);

        var isUsed = await unitOfWork.GetEntityRepository<Room>().DbSet
            .AnyAsync(room => room.LocationId == request.LocationId, cancellationToken);

        if (isUsed)
            return Result.Fail<Unit>(ErrorsCodes.LocationAlreadyUsed);

        await locationRepository.DeleteAsync(location);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(Unit.Value);
    }
}
