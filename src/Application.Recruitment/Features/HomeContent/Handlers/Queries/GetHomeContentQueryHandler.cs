using Application.Recruitment.Features.HomeContent.DTOs;
using Application.Recruitment.Features.HomeContent.Queries;
using Cortex.Mediator.Queries;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.Resources;
using Tawtheef.Domain.Entities;
using Tawtheef.Domain.Entities.Content;

namespace Application.Recruitment.Features.HomeContent.Handlers.Queries;

public sealed class GetHomeContentQueryHandler(IUnitOfWork unitOfWork, IMediaUrlResolver media)
    : IQueryHandler<GetHomeContentQuery, IResult<HomeContentDto>>
{
    public async Task<IResult<HomeContentDto>> Handle(
        GetHomeContentQuery request,
        CancellationToken cancellationToken)
    {
        var stories = await unitOfWork
            .GetEntityRepository<HomeSuccessStory>()
            .DbSet
            .AsNoTracking()
            .Where(story => story.IsActive)
            .OrderBy(story => story.DisplayOrder)
            .ThenBy(story => story.CreatedDate)
            .ToListAsync(cancellationToken);

        var resourceMap = await LoadResourceMapAsync(stories, cancellationToken);

        var successStories = stories.Select(story => new HomeSuccessStoryDto(
            story.Id,
            story.NameAr,
            story.NameEn,
            story.RoleAr,
            story.RoleEn,
            story.MetricTitleAr,
            story.MetricTitleEn,
            story.MetricDescriptionAr,
            story.MetricDescriptionEn,
            ResolveImageUrl(story.ImageUrl, resourceMap),
            story.DisplayOrder)).ToList();

        var faqs = await unitOfWork
            .GetEntityRepository<FAQ>()
            .DbSet
            .AsNoTracking()
            .Where(faq => faq.IsActive)
            .OrderBy(faq => faq.DisplayOrder)
            .ThenBy(faq => faq.CreatedDate)
            .Select(faq => new FAQDto(
                faq.Id,
                faq.QuestionAr,
                faq.QuestionEn,
                faq.AnswerAr,
                faq.AnswerEn,
                faq.DisplayOrder))
            .ToListAsync(cancellationToken);

        return Result.Ok(new HomeContentDto(successStories, faqs));
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
