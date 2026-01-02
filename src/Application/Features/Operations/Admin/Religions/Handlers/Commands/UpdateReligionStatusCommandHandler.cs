using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Features.Operations.Admin.Religions.Commands;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Application.Features.Operations.Admin.Religions.Handlers.Commands;

public sealed class UpdateReligionStatusCommandHandler(
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider,
    ICurrentUserService currentUserService)
    : IRequestHandler<UpdateReligionStatusCommand, IResult<Unit>>
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
