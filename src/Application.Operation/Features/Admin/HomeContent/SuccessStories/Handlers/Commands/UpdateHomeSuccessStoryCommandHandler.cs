using Application.Operation.Features.Admin.HomeContent.SuccessStories.Commands;
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
using Tawtheef.Domain.Entities.Content;

namespace Application.Operation.Features.Admin.HomeContent.SuccessStories.Handlers.Commands;

public sealed class UpdateHomeSuccessStoryCommandHandler(
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider,
    ICurrentUserService currentUserService,
    IMediator mediator)
    : ICommandHandler<UpdateHomeSuccessStoryCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(
        UpdateHomeSuccessStoryCommand request,
        CancellationToken cancellationToken)
    {
        var repository = unitOfWork.GetEntityRepository<HomeSuccessStory>();
        var story = await repository.DbSet.FirstOrDefaultAsync(s => s.Id == request.StoryId, cancellationToken);

        if (story is null)
            return Result.Fail<Unit>(ErrorsCodes.NotFound);

        var hasUser = Guid.TryParse(currentUserService.UserId, out var userId);
        var files = request.Files ?? new List<IFormFile>();
        var imageResult = await UploadImageAsync(request.StoryId, request.ImageFileIndex, files, cancellationToken);
        if (imageResult.IsFailed)
            return Result.Fail<Unit>(imageResult.Errors);

        story.NameAr = request.NameAr.Trim();
        story.NameEn = request.NameEn.Trim();
        story.RoleAr = request.RoleAr.Trim();
        story.RoleEn = request.RoleEn.Trim();
        story.MetricTitleAr = request.MetricTitleAr.Trim();
        story.MetricTitleEn = request.MetricTitleEn.Trim();
        story.MetricDescriptionAr = request.MetricDescriptionAr.Trim();
        story.MetricDescriptionEn = request.MetricDescriptionEn.Trim();
        story.ImageUrl = imageResult.Value ?? request.ImageUrl.Trim();
        story.DisplayOrder = request.DisplayOrder;
        story.IsActive = request.IsActive;
        story.UpdatedDate = timeProvider.GetUtcNow();
        story.UpdatedById = hasUser ? userId : story.UpdatedById;

        await repository.UpdateAsync(story);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(Unit.Value);
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

        var uploadPath = await HomeSuccessStoryImageUploadPathFactory.CreateAsync(storyId, file, false, ct);

        var uploadResult = await mediator.SendCommandAsync<UploadAttachmentCommand, IResult<UploadAttachmentRequest>>(
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
