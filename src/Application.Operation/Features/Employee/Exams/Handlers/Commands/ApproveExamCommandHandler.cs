using Application.Operation.Features.Employee.Exams.Commands;
using Application.Operation.Features.Employee.Exams.Handlers.Queries;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Exams;
using Tawtheef.Domain.Entities.Logger;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Events.Operation.Employee.Exams;

namespace Application.Operation.Features.Employee.Exams.Handlers.Commands;

public sealed class ApproveExamCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    : IRequestHandler<ApproveExamCommand, IResult<Unit>>
{
    public Task<IResult<Unit>> Handle(ApproveExamCommand request, CancellationToken ct)
        => unitOfWork.ExecuteInTransactionAsync<IResult<Unit>>(async token =>
        {
            if (request.ExamId == Guid.Empty || !Guid.TryParse(currentUser.UserId, out var reviewerId))
                return Result.Fail<Unit>(ErrorsCodes.InvalidRequest);

            await unitOfWork.Context.Database.ExecuteSqlRawAsync(
                "DECLARE @result int; EXEC @result = sys.sp_getapplock " +
                "@Resource = {0}, @LockMode = N'Exclusive', @LockOwner = N'Transaction', " +
                "@LockTimeout = 15000; IF @result < 0 THROW 51000, 'Exam approval lock unavailable', 1;",
                [$"Tawtheef.Exam.Workflow.{request.ExamId}"], token);

            var exam = await unitOfWork.GetEntityRepository<Exam>().DbSet
                .FirstOrDefaultAsync(x => x.Id == request.ExamId && x.StatusId == ExamStatusIds.PendingApproval,
                    token);
            if (exam == null)
                return Result.Fail<Unit>(ErrorsCodes.InvalidRequest);

            var jobScope = await unitOfWork.Context.Set<Job>().AsNoTracking()
                .Where(x => x.Id == exam.JobId)
                .Select(x => new { x.ManagementId, x.JobTitleId })
                .FirstOrDefaultAsync(token);
            if (jobScope == null)
                return Result.Fail<Unit>(ErrorsCodes.InvalidRequest);

            var banks = await ExamBankSelection.Load(
                unitOfWork, jobScope.ManagementId, jobScope.JobTitleId, token);
            var categories = await unitOfWork.Context.Set<ExamCategory>().AsNoTracking()
                .Where(x => unitOfWork.Context.Set<ExamPart>()
                    .Where(part => part.ExamId == exam.Id)
                    .Select(part => part.Id)
                    .Contains(x.ExamPartId))
                .ToListAsync(token);
            foreach (var category in categories)
            {
                var bank = banks.FirstOrDefault(x => x.Id == category.QuestionBankVersionId &&
                    x.QuestionBankTypeId == category.QuestionBankTypeId);
                if (bank == null || category.EasyQuestionCount > bank.Easy ||
                    category.MediumQuestionCount > bank.Medium || category.HardQuestionCount > bank.Hard)
                    return Result.Fail<Unit>(ErrorsCodes.InvalidRequest);
            }

            var metadata = new ExamWorkflowMetadata(
                reviewerId,
                DateTime.UtcNow,
                nameof(ExamStatusIds.PendingApproval),
                nameof(ExamStatusIds.Approved));
            metadata.ApplyDecision(exam, ExamStatusIds.Approved);
            await unitOfWork.GetEntityRepository<ActionLog>().AddAsync(
                metadata.CreateActionLog(exam, "ExamApproved"), token);

            if (exam.CreatedById.HasValue)
                exam.AddDomainEvent(new ExamApprovedDomainEvent(exam.CreatedById.Value, exam.Id, exam.ExamNo));

            await unitOfWork.SaveChangesAsync(token);
            return Result.Ok(Unit.Value);
        }, ct);
}
