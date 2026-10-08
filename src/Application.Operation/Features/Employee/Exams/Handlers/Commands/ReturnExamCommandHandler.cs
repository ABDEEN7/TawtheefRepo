using Application.Operation.Features.Employee.Exams.Commands;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Concurrency;
using Tawtheef.Application.Common.Interfaces.Concurrency;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Exams;
using Tawtheef.Domain.Entities.Logger;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Events.Operation.Employee.Exams;

namespace Application.Operation.Features.Employee.Exams.Handlers.Commands;

public sealed class ReturnExamCommandHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    TimeProvider timeProvider,
    IApplicationLockService applicationLockService)
    : IRequestHandler<ReturnExamCommand, IResult<Unit>>
{
    public Task<IResult<Unit>> Handle(ReturnExamCommand request, CancellationToken ct)
        => unitOfWork.ExecuteInTransactionAsync<IResult<Unit>>(async token =>
        {
            if (string.IsNullOrWhiteSpace(request.Note) ||
                !Guid.TryParse(currentUser.UserId, out var reviewerId))
                return Result.Fail<Unit>(ErrorsCodes.InvalidRequest);

            await applicationLockService.AcquireExclusiveAsync(
                ApplicationLockResources.ExamWorkflow(request.ExamId), token);

            var exam = await unitOfWork.GetEntityRepository<Exam>().DbSet
                .FirstOrDefaultAsync(x => x.Id == request.ExamId &&
                    x.StatusId == ExamStatusIds.PendingApproval, token);
            if (exam == null)
                return Result.Fail<Unit>(ErrorsCodes.InvalidRequest);

            var note = request.Note.Trim();
            var reviewedAtUtc = timeProvider.GetUtcNow().UtcDateTime;
            var metadata = new ExamWorkflowMetadata(
                reviewerId,
                reviewedAtUtc,
                nameof(ExamStatusIds.PendingApproval),
                nameof(ExamStatusIds.Returned));
            metadata.ApplyDecision(exam, ExamStatusIds.Returned);
            exam.DecisionNotes = note;

            await unitOfWork.GetEntityRepository<ActionLog>().AddAsync(
                metadata.CreateActionLog(exam, ExamWorkflowAudit.ReturnedForEdit, note), token);

            if (exam.CreatedById.HasValue)
                exam.AddDomainEvent(new ExamReturnedForEditDomainEvent(
                    exam.CreatedById.Value,
                    exam.Id,
                    exam.ExamNo,
                    note));

            await unitOfWork.SaveChangesAsync(token);
            return Result.Ok(Unit.Value);
        }, ct);
}
