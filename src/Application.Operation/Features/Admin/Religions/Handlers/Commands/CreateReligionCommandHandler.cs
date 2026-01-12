using Application.Operation.Features.Admin.Religions.Commands;
using Cortex.Mediator.Commands;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;

namespace Application.Operation.Features.Admin.Religions.Handlers.Commands;

public sealed class CreateReligionCommandHandler(
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider,
    ICurrentUserService currentUserService)
    : ICommandHandler<CreateReligionCommand, IResult<Guid>>
{
    public async Task<IResult<Guid>> Handle(CreateReligionCommand request, CancellationToken cancellationToken)
    {
        var repository = unitOfWork.GetEntityRepository<Religion>();

        var nameAr = request.NameAr.Trim();
        var nameEn = request.NameEn.Trim();

        if (string.IsNullOrWhiteSpace(nameAr) || string.IsNullOrWhiteSpace(nameEn))
            return Result.Fail<Guid>(ErrorsCodes.InvalidName);

        var exists = await repository.DbSet
            .AnyAsync(l => l.NameAr == nameAr || l.NameEn == nameEn, cancellationToken);

        if (exists)
            return Result.Fail<Guid>(ErrorsCodes.ReligionNameExists);

        var now = timeProvider.GetUtcNow();
        var hasUser = Guid.TryParse(currentUserService.UserId, out var userId);

        var newReligion = new Religion
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

        await repository.AddAsync(newReligion);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(newReligion.Id);
    }

    private static string GenerateBackendName(string nameEn)
    {
        var cleaned = nameEn.Trim();
        if (string.IsNullOrWhiteSpace(cleaned))
            return $"REL-{Guid.NewGuid():N}";

        return $"REL-{cleaned.Replace(' ', '-').ToUpperInvariant()}";
    }
}
