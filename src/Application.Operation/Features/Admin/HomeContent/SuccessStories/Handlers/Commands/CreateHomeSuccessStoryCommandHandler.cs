using Application.Operation.Features.Admin.HomeContent.SuccessStories.Commands;
using Cortex.Mediator.Commands;
using FluentResults;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Domain.Entities.Content;

namespace Application.Operation.Features.Admin.HomeContent.SuccessStories.Handlers.Commands;

public sealed class CreateHomeSuccessStoryCommandHandler(
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider,
    ICurrentUserService currentUserService)
    : ICommandHandler<CreateHomeSuccessStoryCommand, IResult<Guid>>
{
    public async Task<IResult<Guid>> Handle(
        CreateHomeSuccessStoryCommand request,
        CancellationToken cancellationToken)
    {
        var repository = unitOfWork.GetEntityRepository<HomeSuccessStory>();
        var hasUser = Guid.TryParse(currentUserService.UserId, out var userId);
        var now = timeProvider.GetUtcNow();

        var newStory = new HomeSuccessStory
        {
            NameAr = request.NameAr.Trim(),
            NameEn = request.NameEn.Trim(),
            RoleAr = request.RoleAr.Trim(),
            RoleEn = request.RoleEn.Trim(),
            MetricTitleAr = request.MetricTitleAr.Trim(),
            MetricTitleEn = request.MetricTitleEn.Trim(),
            MetricDescriptionAr = request.MetricDescriptionAr.Trim(),
            MetricDescriptionEn = request.MetricDescriptionEn.Trim(),
            ImageUrl = request.ImageUrl.Trim(),
            DisplayOrder = request.DisplayOrder,
            IsActive = request.IsActive,
            CreatedDate = now,
            CreatedById = hasUser ? userId : null
        };

        await repository.AddAsync(newStory);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(newStory.Id);
    }
}
