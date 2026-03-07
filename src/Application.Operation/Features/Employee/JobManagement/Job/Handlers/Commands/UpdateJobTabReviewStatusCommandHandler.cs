using Application.Operation.Common.Repositories;
using Application.Operation.Features.Employee.JobManagement.Job.Commands;
using MediatR;
using FluentResults;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;

namespace Application.Operation.Features.Employee.JobManagement.Job.Handlers.Commands;

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

        if (reviewsResult.IsFailed || reviewsResult.Value is null || reviewsResult.Value.Count == 0)
        {
            return Result.Fail<Unit>(JobMessages.ReviewNoteFound);
        }

        reviewsResult.Value.ForEach(r => r.IsResolved = true);

        await uow.SaveChangesAsync(ct);

        return Result.Ok(Unit.Value);
    }
}


