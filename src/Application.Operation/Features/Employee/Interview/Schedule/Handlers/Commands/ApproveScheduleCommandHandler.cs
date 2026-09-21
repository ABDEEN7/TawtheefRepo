using System.Text.Json;
using Application.Operation.Features.Employee.Interview.Schedule.Commands;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.Schedule.Handlers.Commands;

public sealed class ApproveScheduleCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    : IRequestHandler<ApproveScheduleCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(ApproveScheduleCommand request, CancellationToken cancellationToken)
    {
        var schedule = await unitOfWork.GetEntityRepository<InterviewSchedule>().DbSet
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
        if (schedule is null)
            return Result.Fail<Unit>(new Error(ErrorsCodes.InterviewScheduleNotFound));

        _ = Guid.TryParse(currentUserService.UserId, out var approvedById);

        var result = schedule.Approve(approvedById, request.DecisionNotes);
        if (result.IsFailed)
            return Result.Fail<Unit>(result.Errors);

        await unitOfWork.GetEntityRepository<InterviewAuditLog>().AddAsync(new InterviewAuditLog
        {
            EntityType = nameof(InterviewSchedule),
            EntityId = schedule.Id,
            Action = InterviewScheduleAuditActions.Approved,
            NewValues = JsonSerializer.Serialize(new { schedule.ApprovedById, schedule.ApprovedAt, schedule.DecisionNotes })
        }, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(Unit.Value);
    }
}
