using Application.Operation.Features.Admin.HomeContent.Faqs.Commands;
using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Content;

namespace Application.Operation.Features.Admin.HomeContent.Faqs.Handlers.Commands;

public sealed class UpdateFaqStatusCommandHandler(
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider,
    ICurrentUserService currentUserService)
    : ICommandHandler<UpdateFaqStatusCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(
        UpdateFaqStatusCommand request,
        CancellationToken cancellationToken)
    {
        var repository = unitOfWork.GetEntityRepository<FAQ>();
        var faq = await repository.DbSet.FirstOrDefaultAsync(f => f.Id == request.FaqId, cancellationToken);

        if (faq is null)
            return Result.Fail<Unit>(ErrorsCodes.NotFound);

        var hasUser = Guid.TryParse(currentUserService.UserId, out var userId);
        faq.IsActive = request.IsActive;
        faq.UpdatedDate = timeProvider.GetUtcNow().UtcDateTime;
        faq.UpdatedById = hasUser ? userId : faq.UpdatedById;

        await repository.UpdateAsync(faq, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(Unit.Value);
    }
}
