using Application.Operation.Features.Admin.HomeContent.Faqs.Commands;
using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Content;

namespace Application.Operation.Features.Admin.HomeContent.Faqs.Handlers.Commands;

public sealed class DeleteFaqCommandHandler(IUnitOfWork unitOfWork)
    : ICommandHandler<DeleteFaqCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(
        DeleteFaqCommand request,
        CancellationToken cancellationToken)
    {
        var repository = unitOfWork.GetEntityRepository<FAQ>();
        var faq = await repository.DbSet.FirstOrDefaultAsync(f => f.Id == request.FaqId, cancellationToken);

        if (faq is null)
            return Result.Fail<Unit>(ErrorsCodes.NotFound);

        unitOfWork.Remove(faq);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(Unit.Value);
    }
}
