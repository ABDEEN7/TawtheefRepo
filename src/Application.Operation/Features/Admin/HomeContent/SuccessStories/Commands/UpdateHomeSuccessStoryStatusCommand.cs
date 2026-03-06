using MediatR;
using FluentResults;

namespace Application.Operation.Features.Admin.HomeContent.SuccessStories.Commands;

public sealed record UpdateHomeSuccessStoryStatusCommand(Guid StoryId, bool IsActive) : IRequest<IResult<Unit>>;

