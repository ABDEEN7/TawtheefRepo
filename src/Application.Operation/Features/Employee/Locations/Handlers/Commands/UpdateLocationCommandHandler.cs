using Application.Operation.Features.Employee.Locations.Commands;
using FluentResults;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Exams;

namespace Application.Operation.Features.Employee.Locations.Handlers.Commands;

public sealed class UpdateLocationCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateLocationCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(
        UpdateLocationCommand request,
        CancellationToken cancellationToken)
    {
        var locationRepository = unitOfWork.GetEntityRepository<Location>();
        var locationResult = await locationRepository.GetByIdAsync(
            request.LocationId,
            cancellationToken);

        if (locationResult.IsFailed || locationResult.Value is null)
            return Result.Fail<Unit>(ErrorsCodes.LocationNotFound);

        var location = locationResult.Value;
        var updatedLocation = request.Location.Adapt<Location>();

        var duplicateExists = await locationRepository.DbSet.AnyAsync(
            existingLocation => existingLocation.Id != request.LocationId &&
                                existingLocation.LocationLink == updatedLocation.LocationLink &&
                                (existingLocation.NameAr == updatedLocation.NameAr ||
                                 (updatedLocation.NameEn != null &&
                                  existingLocation.NameEn == updatedLocation.NameEn)),
            cancellationToken);

        if (duplicateExists)
            return Result.Fail<Unit>(ErrorsCodes.LocationNameExistsForLink);

        request.Location.Adapt(location);

        await locationRepository.UpdateAsync(location, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(Unit.Value);
    }
}
