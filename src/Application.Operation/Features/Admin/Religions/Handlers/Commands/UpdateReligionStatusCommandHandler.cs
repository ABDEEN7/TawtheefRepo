using Application.Operation.Features.Admin.Religions.Commands;
using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;

namespace Application.Operation.Features.Admin.Religions.Handlers.Commands;

public sealed class UpdateReligionStatusCommandHandler(
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider,
    ICurrentUserService currentUserService)
    : ICommandHandler<UpdateReligionStatusCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(
        UpdateReligionStatusCommand request,
        CancellationToken cancellationToken)
    {
        var repository = unitOfWork.GetEntityRepository<Religion>();
        var religion = await repository.DbSet.FirstOrDefaultAsync(l => l.Id == request.ReligionId, cancellationToken);

        if (religion is null)
            return Result.Fail<Unit>(ErrorsCodes.ReligionNotFound);

        var now = timeProvider.GetUtcNow();
        var hasUser = Guid.TryParse(currentUserService.UserId, out var userId);

        religion.IsActive = request.IsActive;
        religion.UpdatedDate = now;
        religion.UpdatedById = hasUser ? userId : religion.UpdatedById;

        await repository.UpdateAsync(religion);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(Unit.Value);
    }
}
