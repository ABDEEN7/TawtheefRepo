using MediatR;
using FluentResults;

namespace Application.Operation.Features.Admin.HomeContent.SuccessStories.Commands;

public sealed record DeleteHomeSuccessStoryCommand(Guid StoryId) : IRequest<IResult<Unit>>;

