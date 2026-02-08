using Application.Operation.Features.Admin.HomeContent.SuccessStories.DTOs;
using Application.Operation.Features.Admin.HomeContent.SuccessStories.Queries;
using Cortex.Mediator.Queries;
using FluentResults;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Content;

namespace Application.Operation.Features.Admin.HomeContent.SuccessStories.Handlers.Queries;

public sealed class ListHomeSuccessStoriesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
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
            .ProjectToType<HomeSuccessStoryAdminDto>(mapper.Config)
            .ToListAsync(cancellationToken);

        return Result.Ok<IReadOnlyCollection<HomeSuccessStoryAdminDto>>(stories);
    }
}
