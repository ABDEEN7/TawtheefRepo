using Cortex.Mediator.Commands;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Features.Operations.Admin.Religions.Commands;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Application.Features.Operations.Admin.Religions.Handlers.Commands;

public sealed class UpdateReligionCommandHandler(
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider,
    ICurrentUserService currentUserService)
    : ICommandHandler<UpdateReligionCommand, IResult<Guid>>
{
    public async Task<IResult<Guid>> Handle(UpdateReligionCommand request, CancellationToken cancellationToken)
    {
        var repository = unitOfWork.GetEntityRepository<Religion>();

        var nameAr = request.NameAr?.Trim();
        var nameEn = request.NameEn?.Trim();

        if (string.IsNullOrWhiteSpace(nameAr) || string.IsNullOrWhiteSpace(nameEn))
            return Result.Fail<Guid>(ErrorsCodes.InvalidName);

        var exists = await repository.DbSet
            .AnyAsync(
                l => (l.NameAr == nameAr || l.NameEn == nameEn) && l.Id != request.Id,
                cancellationToken);

        if (exists)
            return Result.Fail<Guid>(ErrorsCodes.ReligionNameExists);

        var religion = await repository.DbSet.FirstOrDefaultAsync(l => l.Id == request.Id, cancellationToken);

        if (religion is null)
            return Result.Fail<Guid>(ErrorsCodes.ReligionNotFound);

        var now = timeProvider.GetUtcNow();
        var hasUser = Guid.TryParse(currentUserService.UserId, out var userId);

        religion.NameAr = nameAr!;
        religion.NameEn = nameEn!;
        religion.DescriptionAr = request.DescriptionAr;
        religion.DescriptionEn = request.DescriptionEn;
        religion.IsActive = request.IsActive;
        religion.UpdatedDate = now;
        religion.UpdatedById = hasUser ? userId : religion.UpdatedById;

        await repository.UpdateAsync(religion);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(religion.Id);
    }
}
