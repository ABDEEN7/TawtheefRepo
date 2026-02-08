using Application.Recruitment.Features.HomeContent.DTOs;
using Application.Recruitment.Features.HomeContent.Queries;
using Cortex.Mediator.Queries;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Content;

namespace Application.Recruitment.Features.HomeContent.Handlers.Queries;

public sealed class GetHomeContentQueryHandler(IUnitOfWork unitOfWork)
    : IQueryHandler<GetHomeContentQuery, IResult<HomeContentDto>>
{
    public async Task<IResult<HomeContentDto>> Handle(
        GetHomeContentQuery request,
        CancellationToken cancellationToken)
    {
        var successStories = await unitOfWork
            .GetEntityRepository<HomeSuccessStory>()
            .DbSet
            .AsNoTracking()
            .Where(story => story.IsActive)
            .OrderBy(story => story.DisplayOrder)
            .ThenBy(story => story.CreatedDate)
            .Select(story => new HomeSuccessStoryDto(
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
                story.DisplayOrder))
            .ToListAsync(cancellationToken);

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
}
