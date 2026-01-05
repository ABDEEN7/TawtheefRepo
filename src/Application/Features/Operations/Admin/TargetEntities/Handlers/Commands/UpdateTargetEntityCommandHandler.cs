using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Features.Operations.Admin.TargetEntities.Commands;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Application.Features.Operations.Admin.TargetEntities.Handlers.Commands;

public sealed class UpdateTargetEntityCommandHandler(
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider,
    ICurrentUserService currentUserService)
    : IRequestHandler<UpdateTargetEntityCommand, IResult<Guid>>
{
    public async Task<IResult<Guid>> Handle(UpdateTargetEntityCommand request, CancellationToken cancellationToken)
    {
        var repository = unitOfWork.GetEntityRepository<TargetEntity>();

        var nameAr = request.NameAr?.Trim();
        var nameEn = request.NameEn?.Trim();

        if (string.IsNullOrWhiteSpace(nameAr) || string.IsNullOrWhiteSpace(nameEn))
            return Result.Fail<Guid>(ErrorsCodes.InvalidName);

        var exists = await repository.DbSet.AnyAsync(
            t =>
                t.Id != request.Id &&
                (t.NameAr == nameAr || t.NameEn == nameEn),
            cancellationToken);

        if (exists)
            return Result.Fail<Guid>(ErrorsCodes.TargetEntityNameExists);

        var targetEntity = await repository.DbSet.FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (targetEntity is null)
            return Result.Fail<Guid>(ErrorsCodes.TargetEntityNotFound);

        var now = timeProvider.GetUtcNow();
        var hasUser = Guid.TryParse(currentUserService.UserId, out var userId);

        targetEntity.NameAr = nameAr!;
        targetEntity.NameEn = nameEn!;
        targetEntity.DescriptionAr = request.DescriptionAr;
        targetEntity.DescriptionEn = request.DescriptionEn;
        targetEntity.IsActive = request.IsActive;
        targetEntity.UpdatedDate = now;
        targetEntity.UpdatedById = hasUser ? userId : targetEntity.UpdatedById;

        await repository.UpdateAsync(targetEntity);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(targetEntity.Id);
    }
}
