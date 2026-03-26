using Application.Operation.Features.Admin.HomeContent.SuccessStories.Commands;
using MediatR;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Application.Common.Services;
using Tawtheef.Application.Features.Resources.Commands;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Content;

namespace Application.Operation.Features.Admin.HomeContent.SuccessStories.Handlers.Commands;

public sealed class CreateHomeSuccessStoryCommandHandler(
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider,
    ICurrentUserService currentUserService,
    IMediator mediator)
    : IRequestHandler<CreateHomeSuccessStoryCommand, IResult<Guid>>
{
    public async Task<IResult<Guid>> Handle(
        CreateHomeSuccessStoryCommand request,
        CancellationToken cancellationToken)
    {
        var repository = unitOfWork.GetEntityRepository<HomeSuccessStory>();
        var hasUser = Guid.TryParse(currentUserService.UserId, out var userId);
        var now = timeProvider.GetUtcNow();
        var files = request.Files ?? new List<IFormFile>();
        var storyId = Guid.NewGuid();

        var imageResult = await UploadImageAsync(storyId, request.ImageFileIndex, files, cancellationToken);
        if (imageResult.IsFailed)
            return Result.Fail<Guid>(imageResult.Errors);
        var imageUrl = request.ImageUrl?.Trim() ?? string.Empty;

        var newStory = new HomeSuccessStory
        {
            Id = storyId,
            NameAr = request.NameAr.Trim(),
            NameEn = request.NameEn.Trim(),
            RoleAr = request.RoleAr.Trim(),
            RoleEn = request.RoleEn.Trim(),
            MetricTitleAr = request.MetricTitleAr.Trim(),
            MetricTitleEn = request.MetricTitleEn.Trim(),
            MetricDescriptionAr = request.MetricDescriptionAr.Trim(),
            MetricDescriptionEn = request.MetricDescriptionEn.Trim(),
            ImageUrl = imageResult.Value ?? imageUrl,
            DisplayOrder = request.DisplayOrder,
            IsActive = request.IsActive,
            CreatedDate = now.UtcDateTime,
            CreatedById = hasUser ? userId : null
        };

        await repository.AddAsync(newStory);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(newStory.Id);
    }

    private async Task<Result<string?>> UploadImageAsync(
        Guid storyId,
        int? fileIndex,
        IReadOnlyList<IFormFile> files,
        CancellationToken ct)
    {
        if (fileIndex is null)
            return Result.Ok<string?>(null);

        if (fileIndex.Value < 0 || fileIndex.Value >= files.Count)
            return Result.Fail<string?>(ErrorsCodes.InvalidAttachmentFileIndex);

        var file = files[fileIndex.Value];
        if (file.Length == 0)
            return Result.Fail<string?>(ErrorsCodes.InvalidAttachmentFile);

        var imageValidationResult = HomeSuccessStoryImageValidator.Validate(file);
        if (imageValidationResult.IsFailed)
            return Result.Fail<string?>(imageValidationResult.Errors);

        var uploadPath = await HomeSuccessStoryImageUploadPathFactory.CreateAsync(storyId, file, true, ct);

        var uploadResult = await mediator.Send(
            new UploadAttachmentCommand(
                Guid.TryParse(currentUserService.UserId, out var userId) ? userId : Guid.Empty,
                uploadPath.FileId,
                uploadPath.Path,
                uploadPath.Hash,
                file),
            ct);

        if (uploadResult.IsFailed)
            return Result.Fail<string?>(uploadResult.Errors);

        return Result.Ok<string?>(uploadResult.Value.ResourceId.ToString());
    }
}


