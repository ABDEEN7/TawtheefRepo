using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Features.Operations.Admin.Universities.Commands;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Tawtheef.Application.Features.Operations.Admin.Universities.Handlers.Commands;

public sealed class UpdateUniversityStatusCommandHandler(
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider,
    ICurrentUserService currentUserService)
    : IRequestHandler<UpdateUniversityStatusCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(
        UpdateUniversityStatusCommand request,
        CancellationToken cancellationToken)
    {
        var repository = unitOfWork.GetEntityRepository<University>();
        var university = await repository.DbSet.FirstOrDefaultAsync(u => u.Id == request.UniversityId, cancellationToken);

        if (university is null)
            return Result.Fail<Unit>(ErrorsCodes.UniversityNotFound);

        var now = timeProvider.GetUtcNow();
        var hasUser = Guid.TryParse(currentUserService.UserId, out var userId);

        university.IsActive = request.IsActive;
        university.UpdatedDate = now;
        university.UpdatedById = hasUser ? userId : university.UpdatedById;

        await repository.UpdateAsync(university);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(Unit.Value);
    }
}
