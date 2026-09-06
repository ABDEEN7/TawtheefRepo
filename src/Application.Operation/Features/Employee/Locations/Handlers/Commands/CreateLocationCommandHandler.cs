using Application.Operation.Features.Employee.Locations.Commands;
using FluentResults;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Exams;

namespace Application.Operation.Features.Employee.Locations.Handlers.Commands;

public sealed class CreateLocationCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CreateLocationCommand, IResult<Guid>>
{
    public async Task<IResult<Guid>> Handle(
        CreateLocationCommand request,
        CancellationToken cancellationToken)
    {
        var locationRepository = unitOfWork.GetEntityRepository<Location>();
        var location = request.Location.Adapt<Location>();

        var duplicateExists = await locationRepository.DbSet.AnyAsync(
            existingLocation => existingLocation.LocationLink == location.LocationLink &&
                                (existingLocation.NameAr == location.NameAr ||
                                 (location.NameEn != null &&
                                  existingLocation.NameEn == location.NameEn)),
            cancellationToken);

        if (duplicateExists)
            return Result.Fail<Guid>(ErrorsCodes.LocationNameExistsForLink);

        await locationRepository.AddAsync(location);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(location.Id);
    }
}
