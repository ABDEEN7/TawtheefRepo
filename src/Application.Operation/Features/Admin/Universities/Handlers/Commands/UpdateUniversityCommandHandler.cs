using Application.Operation.Features.Admin.Universities.Commands;
using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Application.Common.Services;
using Tawtheef.Application.Features.Resources.Commands;
using Tawtheef.Application.Features.Resources.DTOs;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Application.Operation.Features.Admin.Universities.Handlers.Commands;

public sealed class UpdateUniversityCommandHandler(
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider,
    ICurrentUserService currentUserService,
    IMediator mediator)
    : ICommandHandler<UpdateUniversityCommand, IResult<Guid>>
{
    public async Task<IResult<Guid>> Handle(UpdateUniversityCommand request, CancellationToken cancellationToken)
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
            u => (u.NameAr == nameAr || u.NameEn == nameEn) && u.Id != request.Id,
            cancellationToken);

        if (exists)
            return Result.Fail<Guid>(ErrorsCodes.UniversityNameExists);

        var university = await universityRepo.DbSet.FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);
        if (university is null)
            return Result.Fail<Guid>(ErrorsCodes.UniversityNotFound);

        var now = timeProvider.GetUtcNow();
        _ = Guid.TryParse(currentUserService.UserId, out var userId);
        var files = request.Files ?? [];

        var logoArResult = await UploadLogoAsync(university.Id, "ar", request.LogoArFileIndex, files, cancellationToken);
        if (logoArResult.IsFailed)
            return Result.Fail<Guid>(logoArResult.Errors);

        var logoEnResult = await UploadLogoAsync(university.Id, "en", request.LogoEnFileIndex, files, cancellationToken);
        if (logoEnResult.IsFailed)
            return Result.Fail<Guid>(logoEnResult.Errors);

        university.NameAr = nameAr;
        university.NameEn = nameEn;
        university.DescriptionAr = request.DescriptionAr;
        university.DescriptionEn = request.DescriptionEn;
        university.CityId = request.CityId;
        university.WebSite = request.WebSite;
        university.Phone = request.Phone;
        university.Email = request.Email;
        university.Code = request.Code;
        university.OriginalName = request.OriginalName;
        university.IsActive = request.IsActive;
        university.UpdatedDate = now;
        university.UpdatedById = userId;
        university.LogoArId = logoArResult.Value ?? university.LogoArId;
        university.LogoEnId = logoEnResult.Value ?? university.LogoEnId;

        await universityRepo.UpdateAsync(university);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(university.Id);
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

        var uploadResult = await mediator.SendCommandAsync<UploadAttachmentCommand, IResult<UploadAttachmentRequest>>(
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
