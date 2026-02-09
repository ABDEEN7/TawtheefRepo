using Application.Operation.Features.Admin.HomeContent.SuccessStories.Commands;
using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Content;

namespace Application.Operation.Features.Admin.HomeContent.SuccessStories.Handlers.Commands;

public sealed class DeleteHomeSuccessStoryCommandHandler(IUnitOfWork unitOfWork)
    : ICommandHandler<DeleteHomeSuccessStoryCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(
        DeleteHomeSuccessStoryCommand request,
        CancellationToken cancellationToken)
    {
        var repository = unitOfWork.GetEntityRepository<HomeSuccessStory>();
        var story = await repository.DbSet.FirstOrDefaultAsync(s => s.Id == request.StoryId, cancellationToken);

        if (story is null)
            return Result.Fail<Unit>(ErrorsCodes.NotFound);

        unitOfWork.Remove(story);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(Unit.Value);
    }
}
