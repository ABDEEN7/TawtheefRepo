using Application.Operation.Features.Admin.TargetEntities.Commands;
using Cortex.Mediator.Commands;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;

namespace Application.Operation.Features.Admin.TargetEntities.Handlers.Commands;

public sealed class CreateTargetEntityCommandHandler(
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider,
    ICurrentUserService currentUserService)
    : ICommandHandler<CreateTargetEntityCommand, IResult<Guid>>
{
    public async Task<IResult<Guid>> Handle(CreateTargetEntityCommand request, CancellationToken cancellationToken)
    {
        var repository = unitOfWork.GetEntityRepository<TargetEntity>();

        var nameAr = request.NameAr.Trim();
        var nameEn = request.NameEn.Trim();

        if (string.IsNullOrWhiteSpace(nameAr) || string.IsNullOrWhiteSpace(nameEn))
            return Result.Fail<Guid>(ErrorsCodes.InvalidName);

        var exists = await repository.DbSet.AnyAsync(
            t =>
                t.NameAr == nameAr ||
                t.NameEn == nameEn,
            cancellationToken);

        if (exists)
            return Result.Fail<Guid>(ErrorsCodes.TargetEntityNameExists);

        var now = timeProvider.GetUtcNow();
        var hasUser = Guid.TryParse(currentUserService.UserId, out var userId);

        var targetEntity = new TargetEntity
        {
            BackendName = GenerateBackendName(nameEn),
            NameAr = nameAr,
            NameEn = nameEn,
            DescriptionAr = request.DescriptionAr,
            DescriptionEn = request.DescriptionEn,
            IsActive = request.IsActive,
            CreatedDate = now,
            CreatedById = hasUser ? userId : null
        };

        await repository.AddAsync(targetEntity);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(targetEntity.Id);
    }

    private static string GenerateBackendName(string nameEn)
    {
        var cleaned = nameEn.Trim();
        if (string.IsNullOrWhiteSpace(cleaned))
            return $"TARGET-{Guid.NewGuid():N}";

        return $"TARGET-{cleaned.Replace(' ', '-').ToUpperInvariant()}";
    }
}
