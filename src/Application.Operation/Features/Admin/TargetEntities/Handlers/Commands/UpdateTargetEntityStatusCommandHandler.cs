using Application.Operation.Features.Admin.TargetEntities.Commands;
using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;

namespace Application.Operation.Features.Admin.TargetEntities.Handlers.Commands;

public sealed class UpdateTargetEntityStatusCommandHandler(
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider,
    ICurrentUserService currentUserService)
    : ICommandHandler<UpdateTargetEntityStatusCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(UpdateTargetEntityStatusCommand request, CancellationToken cancellationToken)
    {
        var repository = unitOfWork.GetEntityRepository<TargetEntity>();
        var targetEntity = await repository.DbSet.FirstOrDefaultAsync(t => t.Id == request.TargetEntityId, cancellationToken);

        if (targetEntity is null)
            return Result.Fail<Unit>(ErrorsCodes.TargetEntityNotFound);

        var now = timeProvider.GetUtcNow();
        var hasUser = Guid.TryParse(currentUserService.UserId, out var userId);

        targetEntity.IsActive = request.IsActive;
        targetEntity.UpdatedDate = now;
        targetEntity.UpdatedById = hasUser ? userId : targetEntity.UpdatedById;

        await repository.UpdateAsync(targetEntity);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(Unit.Value);
    }
}
