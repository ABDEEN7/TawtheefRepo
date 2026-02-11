using Application.Operation.Features.Admin.HomeContent.SuccessStories.DTOs;
using Application.Operation.Features.Admin.HomeContent.SuccessStories.Queries;
using Cortex.Mediator.Queries;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.Resources;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities;
using Tawtheef.Domain.Entities.Content;

namespace Application.Operation.Features.Admin.HomeContent.SuccessStories.Handlers.Queries;

public sealed class GetHomeSuccessStoryDetailsQueryHandler(IUnitOfWork unitOfWork, IMediaUrlResolver media)
    : IQueryHandler<GetHomeSuccessStoryDetailsQuery, IResult<HomeSuccessStoryAdminDto>>
{
    public async Task<IResult<HomeSuccessStoryAdminDto>> Handle(
        GetHomeSuccessStoryDetailsQuery request,
        CancellationToken cancellationToken)
    {
        var story = await unitOfWork
            .GetEntityRepository<HomeSuccessStory>()
            .DbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == request.StoryId, cancellationToken);

        if (story is null)
            return Result.Fail<HomeSuccessStoryAdminDto>(ErrorsCodes.NotFound);

        var imagePreviewUrl = await ResolveImageUrlAsync(story.ImageUrl, cancellationToken);

        return Result.Ok(new HomeSuccessStoryAdminDto(
            story.Id,
            story.NameAr,
            story.NameEn,
            story.RoleAr,
            story.RoleEn,
            story.MetricTitleAr,
            story.MetricTitleEn,
            story.MetricDescriptionAr,
            story.MetricDescriptionEn,
            story.ImageUrl,
            imagePreviewUrl,
            story.DisplayOrder,
            story.IsActive));
    }

    private async Task<string> ResolveImageUrlAsync(string raw, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return string.Empty;
        }

        if (Guid.TryParse(raw, out var id))
        {
            var resource = await unitOfWork
                .GetEntityRepository<Resource>()
                .DbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(item => item.Id == id, ct);

            if (resource is null)
            {
                return string.Empty;
            }

            return media.ResolveAbsolute(resource.Url);
        }

        return media.ResolveAbsolute(raw);
    }
}
