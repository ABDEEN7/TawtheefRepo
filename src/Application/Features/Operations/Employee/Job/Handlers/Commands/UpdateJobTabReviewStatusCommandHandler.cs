using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Operations.Employee.Job.Commands;
using Tawtheef.Domain.Constants;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Handlers.Commands;

public sealed class UpdateJobTabReviewStatusCommandHandler(
    IUnitOfWork uow,
    IJobTabReviewNoteRepository jobTabReviewNoteRepository
) : IRequestHandler<UpdateJobTabReviewStatusCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(
        UpdateJobTabReviewStatusCommand cmd,
        CancellationToken ct)
    {
        var reviewsResult = await jobTabReviewNoteRepository
            .GetByIdWithDetailsAsync(cmd.JobId);

        if (reviewsResult.IsFailed || reviewsResult.Value is null || !reviewsResult.Value.Any())
        {
            return Result.Fail<Unit>(JobMessages.REVIEW_NOTE_FOUND);
        }

        reviewsResult.Value.ForEach(r => r.IsResolved = true);

        await uow.SaveChangesAsync(ct);

        return Result.Ok(Unit.Value);
    }
}

