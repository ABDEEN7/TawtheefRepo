using Application.Operation.Features.Admin.HomeContent.SuccessStories.DTOs;
using Application.Operation.Features.Admin.HomeContent.SuccessStories.Queries;
using Cortex.Mediator.Queries;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.Resources;
using Tawtheef.Domain.Entities;
using Tawtheef.Domain.Entities.Content;

namespace Application.Operation.Features.Admin.HomeContent.SuccessStories.Handlers.Queries;

public sealed class ListHomeSuccessStoriesQueryHandler(IUnitOfWork unitOfWork, IMediaUrlResolver media)
    : IQueryHandler<ListHomeSuccessStoriesQuery, IResult<IReadOnlyCollection<HomeSuccessStoryAdminDto>>>
{
    public async Task<IResult<IReadOnlyCollection<HomeSuccessStoryAdminDto>>> Handle(
        ListHomeSuccessStoriesQuery request,
        CancellationToken cancellationToken)
    {
        var stories = await unitOfWork
            .GetEntityRepository<HomeSuccessStory>()
            .DbSet
            .AsNoTracking()
            .OrderBy(s => s.DisplayOrder)
            .ThenBy(s => s.CreatedDate)
            .ToListAsync(cancellationToken);

        var resourceMap = await LoadResourceMapAsync(stories, cancellationToken);

        var result = stories.Select(story => new HomeSuccessStoryAdminDto(
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
            ResolveImageUrl(story.ImageUrl, resourceMap),
            story.DisplayOrder,
            story.IsActive)).ToList();

        return Result.Ok<IReadOnlyCollection<HomeSuccessStoryAdminDto>>(result);
    }

    private async Task<Dictionary<Guid, string>> LoadResourceMapAsync(
        IReadOnlyCollection<HomeSuccessStory> stories,
        CancellationToken ct)
    {
        var ids = stories
            .Select(story => story.ImageUrl)
            .Select(raw => Guid.TryParse(raw, out var parsed) ? parsed : (Guid?)null)
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .Distinct()
            .ToList();

        if (ids.Count == 0)
        {
            return [];
        }

        return await unitOfWork
            .GetEntityRepository<Resource>()
            .DbSet
            .AsNoTracking()
            .Where(resource => ids.Contains(resource.Id))
            .ToDictionaryAsync(resource => resource.Id, resource => resource.Url, ct);
    }

    private string ResolveImageUrl(string raw, IReadOnlyDictionary<Guid, string> resourceMap)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return string.Empty;
        }

        if (Guid.TryParse(raw, out var id) && resourceMap.TryGetValue(id, out var url))
        {
            return media.ResolveAbsolute(url);
        }

        return media.ResolveAbsolute(raw);
    }
}
