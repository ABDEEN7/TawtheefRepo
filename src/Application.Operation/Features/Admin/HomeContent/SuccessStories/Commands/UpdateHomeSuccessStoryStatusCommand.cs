using Cortex.Mediator.Commands;
using FluentResults;

namespace Application.Operation.Features.Admin.HomeContent.SuccessStories.Commands;

public sealed record UpdateHomeSuccessStoryStatusCommand(Guid StoryId, bool IsActive) : ICommand<IResult<Unit>>;
