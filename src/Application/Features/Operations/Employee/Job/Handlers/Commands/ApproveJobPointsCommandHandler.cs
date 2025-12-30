using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Operations.Employee.Job.Commands;
using Tawtheef.Domain.Constants;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Handlers.Commands;

public class ApproveJobPointsCommandHandler(
    IJobPointsRepository jobPointsRepository,
    IUnitOfWork uow) : IRequestHandler<ApproveJobPointsCommand, IResult<bool>>
{
    public async Task<IResult<bool>> Handle(ApproveJobPointsCommand request, CancellationToken cancellationToken)
    {
        var jobPoints = await jobPointsRepository.GetByJobIdAsync(request.JobId);
        if (jobPoints.IsFailed)
            return Result.Fail<bool>(JobMessages.JobPointsNotFound);

        var points = jobPoints.Value;
        points.IsApproved = true;

        var result = await uow.SaveChangesAsync(cancellationToken);
        return Result.Ok(result > 0);
    }
}
