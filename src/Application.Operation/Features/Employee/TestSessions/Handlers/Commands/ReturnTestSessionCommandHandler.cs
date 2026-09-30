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

public sealed class ReturnTestSessionCommandHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser) : IRequestHandler<ReturnTestSessionCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(ReturnTestSessionCommand request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.DecisionNote) ||
            !Guid.TryParse(currentUser.UserId, out var reviewerId))
            return Result.Fail<Unit>(ErrorsCodes.InvalidRequest);

        var strategy = unitOfWork.Context.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await unitOfWork.Context.Database.BeginTransactionAsync(ct);
            await unitOfWork.Context.Database.ExecuteSqlRawAsync(
                "DECLARE @result int; EXEC @result = sys.sp_getapplock @Resource = {0}, " +
                "@LockMode = N'Exclusive', @LockOwner = N'Transaction', @LockTimeout = 15000; " +
                "IF @result < 0 THROW 51000, 'Test session workflow lock unavailable', 1;",
                [$"Tawtheef.TestSession.Workflow.{request.TestSessionId}"], ct);

            var session = await unitOfWork.Context.Set<TestSession>().FirstOrDefaultAsync(
                x => x.Id == request.TestSessionId && x.StatusId == TestSessionStatusIds.PendingApproval, ct);
            if (session is null)
                return Result.Fail<Unit>(ErrorsCodes.InvalidRequest);

            var note = request.DecisionNote.Trim();
            var metadata = new TestSessionWorkflowMetadata(
                reviewerId,
                DateTime.UtcNow,
                nameof(TestSessionStatusIds.PendingApproval),
                nameof(TestSessionStatusIds.Returned));
            metadata.ApplyDecision(session, TestSessionStatusIds.Returned);
            await unitOfWork.GetEntityRepository<ActionLog>().AddAsync(
                metadata.CreateActionLog(session, "TestSessionReturnedForEdit", note), ct);

            if (session.CreatedById.HasValue)
                session.AddDomainEvent(new TestSessionReturnedForEditDomainEvent(
                    session.CreatedById.Value, session.Id, session.SessionNo, note));

            await unitOfWork.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
            return Result.Ok(Unit.Value);
        });
    }
}
