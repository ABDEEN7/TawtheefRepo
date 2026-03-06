using Application.Operation.Features.Admin.HomeContent.SuccessStories.DTOs;
using MediatR;
using FluentResults;

namespace Application.Operation.Features.Admin.HomeContent.SuccessStories.Queries;

public sealed record GetHomeSuccessStoryDetailsQuery(Guid StoryId) : IRequest<IResult<HomeSuccessStoryAdminDto>>;

