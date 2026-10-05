using Application.Operation.Features.Employee.TestSessions.Commands;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Exams;
using Tawtheef.Domain.Entities.Logger;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Events.Operation.Employee.TestSessions;

namespace Application.Operation.Features.Employee.TestSessions.Handlers.Commands;

public sealed class RejectTestSessionCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    : IRequestHandler<RejectTestSessionCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(RejectTestSessionCommand request, CancellationToken ct)
    {
        if (request.TestSessionId == Guid.Empty || string.IsNullOrWhiteSpace(request.DecisionNote) ||
            !Guid.TryParse(currentUser.UserId, out var reviewerId))
            return Result.Fail<Unit>(ErrorsCodes.InvalidRequest);

        var session = await unitOfWork.Context.Set<TestSession>().FirstOrDefaultAsync(
            x => x.Id == request.TestSessionId && x.StatusId == TestSessionStatusIds.PendingApproval, ct);
        if (session is null)
            return Result.Fail<Unit>(ErrorsCodes.InvalidRequest);

        var note = request.DecisionNote.Trim();
        var metadata = new TestSessionWorkflowMetadata(
            reviewerId,
            DateTime.UtcNow,
            nameof(TestSessionStatusIds.PendingApproval),
            nameof(TestSessionStatusIds.Rejected));
        metadata.ApplyDecision(session, TestSessionStatusIds.Rejected);
        await unitOfWork.GetEntityRepository<ActionLog>().AddAsync(
            metadata.CreateActionLog(session, "TestSessionRejected", note), ct);

        if (session.CreatedById.HasValue)
            session.AddDomainEvent(new TestSessionRejectedDomainEvent(
                session.CreatedById.Value, session.Id, session.SessionNo, note));

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Ok(Unit.Value);
    }
}
