using Cortex.Mediator.Commands;
using FluentResults;

namespace Application.Operation.Features.Admin.HomeContent.SuccessStories.Commands;

public sealed record DeleteHomeSuccessStoryCommand(Guid StoryId) : ICommand<IResult<Unit>>;
