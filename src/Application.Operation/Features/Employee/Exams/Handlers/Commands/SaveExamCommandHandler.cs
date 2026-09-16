using Application.Operation.Features.Employee.Exams.Commands;
using Application.Operation.Features.Employee.Exams.DTOs;
using Application.Operation.Features.Employee.Exams.Handlers.Queries;
using Application.Operation.Features.Employee.Exams.Services;
using FluentResults;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Exams;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Events.Operation.Employee.Exams;

namespace Application.Operation.Features.Employee.Exams.Handlers.Commands;

public sealed class SaveExamCommandHandler(IUnitOfWork unitOfWork, ExamService examService)
    : IRequestHandler<SaveExamCommand, IResult<SavedExamDto>>
{
    public Task<IResult<SavedExamDto>> Handle(SaveExamCommand request, CancellationToken ct)
        => unitOfWork.ExecuteInTransactionAsync<IResult<SavedExamDto>>(async token =>
        {
            // Serialize only exams for this job so the pending-approval rule is safe across instances.
            // The lock is transaction-owned and released on commit/rollback.
            await unitOfWork.Context.Database.ExecuteSqlRawAsync(
                "DECLARE @result int; EXEC @result = sys.sp_getapplock " +
                "@Resource = {0}, @LockMode = N'Exclusive', " +
                "@LockOwner = N'Transaction', @LockTimeout = 15000; " +
                "IF @result < 0 THROW 51000, 'Exam job lock unavailable', 1;",
                [$"Tawtheef.Exam.Job.{request.Exam.JobId}"], token);

            var jobScope = await unitOfWork.Context.Set<Job>().AsNoTracking()
                .Where(x => x.Id == request.Exam.JobId)
                .Select(x => new
                {
                    x.ManagementId,
                    x.JobTitleId,
                    JobTitleAr = x.JobTitle!.JobNameAr,
                    JobTitleEn = x.JobTitle!.JobNameEn,
                })
                .FirstOrDefaultAsync(token);
            if (jobScope == null || !await unitOfWork.Context.Set<ExamInterruptionPolicy>()
                    .AnyAsync(x => x.Id == request.Exam.InterruptionPolicyId, token))
                return Result.Fail<SavedExamDto>(ErrorsCodes.InvalidRequest);

            if (await examService.HasPendingApprovalExamForJobAsync(
                    request.Exam.JobId, request.DraftId, token))
                return Result.Fail<SavedExamDto>(ErrorsCodes.ExamPendingApprovalAlreadyExistsForJob);

            var categoryTypeIds = await unitOfWork.Context.Set<QuestionBankType>().AsNoTracking()
                .Select(x => x.Id).ToListAsync(token);
            var partTwo = request.Exam.Parts[1];
            if (!categoryTypeIds.Contains(QuestionBankTypeIds.Specialized) ||
                partTwo.Categories.Count > categoryTypeIds.Count(id => id != QuestionBankTypeIds.Specialized) ||
                partTwo.Categories.Any(category => !categoryTypeIds.Contains(category.QuestionBankTypeId)))
                return Result.Fail<SavedExamDto>(ErrorsCodes.InvalidRequest);

            var categories = request.Exam.Parts.SelectMany(x => x.Categories).ToList();
            var banks = await ExamBankSelection.Load(
                unitOfWork, jobScope.ManagementId, jobScope.JobTitleId, token);
            foreach (var category in categories)
            {
                var bank = banks.FirstOrDefault(x => x.Id == category.QuestionBankVersionId &&
                    x.QuestionBankTypeId == category.QuestionBankTypeId);
                if (bank == null || (request.Submit &&
                    (category.EasyQuestionCount > bank.Easy ||
                     category.MediumQuestionCount > bank.Medium ||
                     category.HardQuestionCount > bank.Hard)))
                    return Result.Fail<SavedExamDto>(ErrorsCodes.InvalidRequest);
            }

            var repository = unitOfWork.GetEntityRepository<Exam>();
            Exam exam;
            if (request.DraftId is { } draftId)
            {
                var existing = await repository.DbSet.FirstOrDefaultAsync(x => x.Id == draftId, token);
                if (existing == null || (existing.StatusId != ExamStatusIds.Draft &&
                    existing.StatusId != ExamStatusIds.Returned))
                    return Result.Fail<SavedExamDto>(ErrorsCodes.InvalidRequest);
                exam = existing;
                request.Exam.Adapt(exam);
                var oldParts = await unitOfWork.Context.Set<ExamPart>()
                    .Where(x => x.ExamId == exam.Id).ToListAsync(token);
                var oldPartIds = oldParts.Select(x => x.Id).ToList();
                var oldCategories = await unitOfWork.Context.Set<ExamCategory>()
                    .Where(x => oldPartIds.Contains(x.ExamPartId)).ToListAsync(token);
                unitOfWork.RemoveRange(oldCategories);
                unitOfWork.RemoveRange(oldParts);
            }
            else
            {
                exam = request.Exam.Adapt<Exam>();
                // Number allocation remains globally serialized without blocking edits/submissions for other jobs.
                await unitOfWork.Context.Database.ExecuteSqlRawAsync(
                    "DECLARE @result int; EXEC @result = sys.sp_getapplock " +
                    "@Resource = N'Tawtheef.Exam.Numbering', @LockMode = N'Exclusive', " +
                    "@LockOwner = N'Transaction', @LockTimeout = 15000; " +
                    "IF @result < 0 THROW 51000, 'Exam numbering lock unavailable', 1;", token);
                var prefix = $"EX-{DateTime.UtcNow.Year}-";
                var numbers = await repository.DbSet.IgnoreQueryFilters().AsNoTracking()
                    .Where(x => x.ExamNo.StartsWith(prefix)).Select(x => x.ExamNo).ToListAsync(token);
                var sequence = numbers.Select(x => long.TryParse(x[prefix.Length..], out var n) ? n : 0)
                    .DefaultIfEmpty().Max() + 1;
                exam.ExamNo = $"{prefix}{sequence:D4}";
                var added = await repository.AddAsync(exam);
                if (added.IsFailed) return Result.Fail<SavedExamDto>(ErrorsCodes.InvalidRequest);
            }

            exam.TotalQuestions = checked((int)categories.Sum(x => (long)x.QuestionCount));
            if (request.Submit)
                exam.StatusId = ExamStatusIds.PendingApproval;
            else if (!request.DraftId.HasValue)
                exam.StatusId = ExamStatusIds.Draft;
            foreach (var partDto in request.Exam.Parts)
            {
                var part = partDto.Adapt<ExamPart>();
                part.Exam = exam;
                var addedPart = await unitOfWork.GetEntityRepository<ExamPart>().AddAsync(part);
                if (addedPart.IsFailed) return Result.Fail<SavedExamDto>(ErrorsCodes.InvalidRequest);
                foreach (var categoryDto in partDto.Categories)
                {
                    var category = categoryDto.Adapt<ExamCategory>();
                    category.ExamPart = part;
                    var addedCategory = await unitOfWork.GetEntityRepository<ExamCategory>().AddAsync(category);
                    if (addedCategory.IsFailed) return Result.Fail<SavedExamDto>(ErrorsCodes.InvalidRequest);
                }
            }
            if (request.Submit)
            {
                exam.AddDomainEvent(new ExamSubmittedForApprovalDomainEvent(
                    exam.Id,
                    exam.ExamNo,
                    exam.TitleAr,
                    exam.TitleEn,
                    jobScope.JobTitleAr,
                    jobScope.JobTitleEn));
            }
            await unitOfWork.SaveChangesAsync(token);
            return Result.Ok(new SavedExamDto(exam.Id, exam.ExamNo));
        }, ct);
}
