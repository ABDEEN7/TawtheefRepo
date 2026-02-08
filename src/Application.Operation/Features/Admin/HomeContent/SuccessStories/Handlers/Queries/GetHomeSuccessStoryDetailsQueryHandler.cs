using Application.Operation.Features.Admin.HomeContent.SuccessStories.DTOs;
using Application.Operation.Features.Admin.HomeContent.SuccessStories.Queries;
using Cortex.Mediator.Queries;
using FluentResults;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Content;

namespace Application.Operation.Features.Admin.HomeContent.SuccessStories.Handlers.Queries;

public sealed class GetHomeSuccessStoryDetailsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
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

        return Result.Ok(mapper.Map<HomeSuccessStoryAdminDto>(story));
    }
}
