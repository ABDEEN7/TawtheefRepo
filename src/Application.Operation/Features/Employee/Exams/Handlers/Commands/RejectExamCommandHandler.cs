using Application.Operation.Features.Employee.Exams.Commands;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Exams;
using Tawtheef.Domain.Entities.Logger;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Events.Operation.Employee.Exams;

namespace Application.Operation.Features.Employee.Exams.Handlers.Commands;

public sealed class RejectExamCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    : IRequestHandler<RejectExamCommand, IResult<Unit>>
{
    public Task<IResult<Unit>> Handle(RejectExamCommand request, CancellationToken ct)
        => unitOfWork.ExecuteInTransactionAsync<IResult<Unit>>(async token =>
        {
            if (request.ExamId == Guid.Empty || string.IsNullOrWhiteSpace(request.Note) ||
                !Guid.TryParse(currentUser.UserId, out var reviewerId))
                return Result.Fail<Unit>(ErrorsCodes.InvalidRequest);

            // Use the established approval lock so competing terminal decisions cannot both succeed.
            await unitOfWork.Context.Database.ExecuteSqlRawAsync(
                "DECLARE @result int; EXEC @result = sys.sp_getapplock " +
                "@Resource = {0}, @LockMode = N'Exclusive', @LockOwner = N'Transaction', " +
                "@LockTimeout = 15000; IF @result < 0 THROW 51000, 'Exam workflow lock unavailable', 1;",
                [$"Tawtheef.Exam.Workflow.{request.ExamId}"], token);

            var exam = await unitOfWork.GetEntityRepository<Exam>().DbSet
                .FirstOrDefaultAsync(x => x.Id == request.ExamId && x.StatusId == ExamStatusIds.PendingApproval,
                    token);
            if (exam == null)
                return Result.Fail<Unit>(ErrorsCodes.InvalidRequest);

            var note = request.Note.Trim();
            var metadata = new ExamWorkflowMetadata(
                reviewerId,
                DateTime.UtcNow,
                nameof(ExamStatusIds.PendingApproval),
                nameof(ExamStatusIds.Rejected));
            metadata.ApplyDecision(exam, ExamStatusIds.Rejected);
            exam.DecisionNotes = note;

            await unitOfWork.GetEntityRepository<ActionLog>().AddAsync(
                metadata.CreateActionLog(exam, "ExamRejected", note), token);

            if (exam.CreatedById.HasValue)
                exam.AddDomainEvent(new ExamRejectedDomainEvent(exam.CreatedById.Value, exam.Id, exam.ExamNo, note));

            await unitOfWork.SaveChangesAsync(token);
            return Result.Ok(Unit.Value);
        }, ct);
}
