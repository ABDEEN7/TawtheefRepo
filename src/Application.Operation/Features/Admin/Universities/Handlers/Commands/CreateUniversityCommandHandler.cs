using Application.Operation.Features.Admin.Universities.Commands;
using MediatR;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Application.Common.Services;
using Tawtheef.Application.Features.Resources.Commands;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Application.Operation.Features.Admin.Universities.Handlers.Commands;

public sealed class CreateUniversityCommandHandler(
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider,
    ICurrentUserService currentUserService,
    IMediator mediator)
    : IRequestHandler<CreateUniversityCommand, IResult<Guid>>
{
    public async Task<IResult<Guid>> Handle(CreateUniversityCommand request, CancellationToken cancellationToken)
    {
        var universityRepo = unitOfWork.GetEntityRepository<University>();
        var cityRepo = unitOfWork.GetEntityRepository<City>();

        var nameAr = request.NameAr.Trim();
        var nameEn = request.NameEn.Trim();

        if (string.IsNullOrWhiteSpace(nameAr) || string.IsNullOrWhiteSpace(nameEn))
            return Result.Fail<Guid>(ErrorsCodes.InvalidName);

        var city = await cityRepo.DbSet
            .Include(c => c.Country)
            .FirstOrDefaultAsync(c => c.Id == request.CityId, cancellationToken);

        if (city is null)
            return Result.Fail<Guid>(ErrorsCodes.CityNotFound);

        if (city.CountryId != request.CountryId)
            return Result.Fail<Guid>(ErrorsCodes.CountryNotFound);

        var exists = await universityRepo.DbSet.AnyAsync(
            u => u.NameAr == nameAr || u.NameEn == nameEn,
            cancellationToken);

        if (exists)
            return Result.Fail<Guid>(ErrorsCodes.UniversityNameExists);

        var now = timeProvider.GetUtcNow().UtcDateTime;
        _ = Guid.TryParse(currentUserService.UserId, out var userId);
        var files = request.Files;
        var universityId = Guid.NewGuid();

        var logoArResult = await UploadLogoAsync(universityId, "ar", request.LogoArFileIndex, files, cancellationToken);
        if (logoArResult.IsFailed)
            return Result.Fail<Guid>(logoArResult.Errors);

        var logoEnResult = await UploadLogoAsync(universityId, "en", request.LogoEnFileIndex, files, cancellationToken);
        if (logoEnResult.IsFailed)
            return Result.Fail<Guid>(logoEnResult.Errors);

        var newUniversity = new University
        {
            Id = universityId,
            BackendName = GenerateBackendName(nameEn),
            NameAr = nameAr,
            NameEn = nameEn,
            DescriptionAr = request.DescriptionAr,
            DescriptionEn = request.DescriptionEn,
            CityId = request.CityId,
            WebSite = request.WebSite,
            Phone = request.Phone,
            Email = request.Email,
            Code = request.Code,
            OriginalName = request.OriginalName,
            LogoArId = logoArResult.Value,
            LogoEnId = logoEnResult.Value,
            IsActive = request.IsActive,
            CreatedDate = now,
            CreatedById = userId
        };

        await universityRepo.AddAsync(newUniversity);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(newUniversity.Id);
    }

    private static string GenerateBackendName(string nameEn)
    {
        var cleaned = nameEn.Trim();
        return string.IsNullOrWhiteSpace(cleaned)
            ? $"UNIV-{Guid.NewGuid():N}"
            : $"UNIV-{cleaned.Replace(' ', '-').ToUpperInvariant()}";
    }

    private async Task<Result<Guid?>> UploadLogoAsync(
        Guid universityId,
        string logoType,
        int? fileIndex,
        IReadOnlyList<IFormFile> files,
        CancellationToken ct)
    {
        if (fileIndex is null)
            return Result.Ok<Guid?>(null);

        if (fileIndex.Value < 0 || fileIndex.Value >= files.Count)
            return Result.Fail<Guid?>(ErrorsCodes.InvalidAttachmentFileIndex);

        var file = files[fileIndex.Value];
        if (file.Length == 0)
            return Result.Fail<Guid?>(ErrorsCodes.InvalidAttachmentFile);

        var uploadPath = await UniversityLogoUploadPathFactory.CreateAsync(universityId, logoType, file, false, ct);

        var uploadResult = await mediator.Send(
            new UploadAttachmentCommand(
                Guid.TryParse(currentUserService.UserId, out var userId) ? userId : Guid.Empty,
                uploadPath.FileId,
                uploadPath.Path,
                uploadPath.Hash,
                file),
            ct);

        if (uploadResult.IsFailed)
            return Result.Fail<Guid?>(uploadResult.Errors);

        return Result.Ok<Guid?>(uploadResult.Value.ResourceId);
    }
}


