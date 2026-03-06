using Application.Operation.Features.Admin.TargetEntities.Commands;
using MediatR;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;

namespace Application.Operation.Features.Admin.TargetEntities.Handlers.Commands;

public sealed class UpdateTargetEntityStatusCommandHandler(
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider,
    ICurrentUserService currentUserService)
    : IRequestHandler<UpdateTargetEntityStatusCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(UpdateTargetEntityStatusCommand request, CancellationToken cancellationToken)
    {
        var repository = unitOfWork.GetEntityRepository<TargetEntity>();
        var targetEntity = await repository.DbSet.FirstOrDefaultAsync(t => t.Id == request.TargetEntityId, cancellationToken);

        if (targetEntity is null)
            return Result.Fail<Unit>(ErrorsCodes.TargetEntityNotFound);

        var now = timeProvider.GetUtcNow().UtcDateTime;
        var hasUser = Guid.TryParse(currentUserService.UserId, out var userId);

        targetEntity.IsActive = request.IsActive;
        targetEntity.UpdatedDate = now;
        targetEntity.UpdatedById = hasUser ? userId : targetEntity.UpdatedById;

        await repository.UpdateAsync(targetEntity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(Unit.Value);
    }
}

