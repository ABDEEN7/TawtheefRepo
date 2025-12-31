using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Features.Operations.Admin.Languages.Commands;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Application.Features.Operations.Admin.Languages.Handlers.Commands;

public sealed class SaveLanguageCommandHandler(
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider,
    ICurrentUserService currentUserService)
    : IRequestHandler<SaveLanguageCommand, IResult<Guid>>
{
    public async Task<IResult<Guid>> Handle(
        SaveLanguageCommand request,
        CancellationToken cancellationToken)
    {
        var repository = unitOfWork.GetEntityRepository<Language>();

        var nameAr = request.NameAr?.Trim();
        var nameEn = request.NameEn?.Trim();

        if (string.IsNullOrWhiteSpace(nameAr) || string.IsNullOrWhiteSpace(nameEn))
            return Result.Fail<Guid>(ErrorsCodes.InvalidName);

        var exists = await repository.DbSet
            .AnyAsync(l =>
                (l.NameAr == nameAr || l.NameEn == nameEn) &&
                l.Id != request.Id, cancellationToken);

        if (exists)
            return Result.Fail<Guid>(ErrorsCodes.LanguageNameExists);

        var now = timeProvider.GetUtcNow();
        var hasUser = Guid.TryParse(currentUserService.UserId, out var userId);

        if (request.Id.HasValue)
        {
            var language = await repository.DbSet.FirstOrDefaultAsync(l => l.Id == request.Id.Value, cancellationToken);
            if (language is null)
                return Result.Fail<Guid>(ErrorsCodes.LanguageNotFound);

            language.NameAr = nameAr!;
            language.NameEn = nameEn!;
            language.DescriptionAr = request.DescriptionAr;
            language.DescriptionEn = request.DescriptionEn;
            language.IsActive = request.IsActive;
            language.UpdatedDate = now;
            language.UpdatedById = hasUser ? userId : language.UpdatedById;

            await repository.UpdateAsync(language);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Ok(language.Id);
        }

        var newLanguage = new Language
        {
            BackendName = GenerateBackendName(nameEn!),
            NameAr = nameAr!,
            NameEn = nameEn!,
            DescriptionAr = request.DescriptionAr,
            DescriptionEn = request.DescriptionEn,
            IsActive = request.IsActive,
            CreatedDate = now,
            CreatedById = hasUser ? userId : null
        };

        await repository.AddAsync(newLanguage);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(newLanguage.Id);
    }

    private static string GenerateBackendName(string nameEn)
    {
        var cleaned = nameEn.Trim();
        if (string.IsNullOrWhiteSpace(cleaned))
            return $"LANG-{Guid.NewGuid():N}";

        return $"LANG-{cleaned.Replace(' ', '-').ToUpperInvariant()}";
    }
}
