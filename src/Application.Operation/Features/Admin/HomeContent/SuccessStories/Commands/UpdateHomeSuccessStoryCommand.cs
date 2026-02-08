using Cortex.Mediator.Commands;
using FluentResults;

namespace Application.Operation.Features.Admin.HomeContent.SuccessStories.Commands;

public sealed record UpdateHomeSuccessStoryCommand(
    Guid StoryId,
    string NameAr,
    string NameEn,
    string RoleAr,
    string RoleEn,
    string MetricTitleAr,
    string MetricTitleEn,
    string MetricDescriptionAr,
    string MetricDescriptionEn,
    string ImageUrl,
    int DisplayOrder,
    bool IsActive) : ICommand<IResult<Unit>>;
