using Application.Operation.Features.Admin.HomeContent.SuccessStories.DTOs;
using Cortex.Mediator.Queries;
using FluentResults;

namespace Application.Operation.Features.Admin.HomeContent.SuccessStories.Queries;

public sealed record GetHomeSuccessStoryDetailsQuery(Guid StoryId) : IQuery<IResult<HomeSuccessStoryAdminDto>>;
