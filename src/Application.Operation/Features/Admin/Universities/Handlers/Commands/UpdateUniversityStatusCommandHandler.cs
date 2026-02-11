using Application.Operation.Features.Admin.Universities.Commands;
using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Application.Operation.Features.Admin.Universities.Handlers.Commands;

public sealed class UpdateUniversityStatusCommandHandler(
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider,
    ICurrentUserService currentUserService)
    : ICommandHandler<UpdateUniversityStatusCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(
        UpdateUniversityStatusCommand request,
        CancellationToken cancellationToken)
    {
        var repository = unitOfWork.GetEntityRepository<University>();
        var university = await repository.DbSet.FirstOrDefaultAsync(u => u.Id == request.UniversityId, cancellationToken);

        if (university is null)
            return Result.Fail<Unit>(ErrorsCodes.UniversityNotFound);

        var now = timeProvider.GetUtcNow().UtcDateTime;
        var hasUser = Guid.TryParse(currentUserService.UserId, out var userId);

        university.IsActive = request.IsActive;
        university.UpdatedDate = now;
        university.UpdatedById = hasUser ? userId : university.UpdatedById;

        await repository.UpdateAsync(university);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(Unit.Value);
    }
}
