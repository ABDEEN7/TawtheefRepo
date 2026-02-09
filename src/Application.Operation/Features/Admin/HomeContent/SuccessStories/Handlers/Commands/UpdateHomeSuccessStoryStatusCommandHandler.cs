using Application.Operation.Features.Admin.HomeContent.SuccessStories.Commands;
using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Content;

namespace Application.Operation.Features.Admin.HomeContent.SuccessStories.Handlers.Commands;

public sealed class UpdateHomeSuccessStoryStatusCommandHandler(
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider,
    ICurrentUserService currentUserService)
    : ICommandHandler<UpdateHomeSuccessStoryStatusCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(
        UpdateHomeSuccessStoryStatusCommand request,
        CancellationToken cancellationToken)
    {
        var repository = unitOfWork.GetEntityRepository<HomeSuccessStory>();
        var story = await repository.DbSet.FirstOrDefaultAsync(s => s.Id == request.StoryId, cancellationToken);

        if (story is null)
            return Result.Fail<Unit>(ErrorsCodes.NotFound);

        var hasUser = Guid.TryParse(currentUserService.UserId, out var userId);
        story.IsActive = request.IsActive;
        story.UpdatedDate = timeProvider.GetUtcNow();
        story.UpdatedById = hasUser ? userId : story.UpdatedById;

        await repository.UpdateAsync(story);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(Unit.Value);
    }
}
