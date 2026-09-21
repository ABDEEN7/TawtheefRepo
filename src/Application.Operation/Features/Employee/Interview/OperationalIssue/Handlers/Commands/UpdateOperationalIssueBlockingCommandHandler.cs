using System.Text.Json;
using Application.Operation.Features.Employee.Interview.OperationalIssue.Commands;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.OperationalIssue.Handlers.Commands;

public sealed class UpdateOperationalIssueBlockingCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateOperationalIssueBlockingCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(UpdateOperationalIssueBlockingCommand request, CancellationToken cancellationToken)
    {
        var issue = await unitOfWork.GetEntityRepository<InterviewOperationalIssue>().DbSet
            .FirstOrDefaultAsync(i => i.Id == request.IssueId, cancellationToken);
        if (issue is null)
            return Result.Fail<Unit>(new Error(ErrorsCodes.InterviewOperationalIssueNotFound));

        var result = issue.UpdateBlocking(request.IsBlocking);
        if (result.IsFailed)
            return Result.Fail<Unit>(result.Errors);

        await unitOfWork.GetEntityRepository<InterviewAuditLog>().AddAsync(new InterviewAuditLog
        {
            EntityType = nameof(InterviewOperationalIssue),
            EntityId = issue.Id,
            Action = InterviewOperationalIssueAuditActions.BlockingUpdated,
            NewValues = JsonSerializer.Serialize(new { request.IsBlocking })
        }, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(Unit.Value);
    }
}
