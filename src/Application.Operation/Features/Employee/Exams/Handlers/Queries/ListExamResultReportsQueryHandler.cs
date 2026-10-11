using Application.Operation.Features.Employee.Exams.DTOs;
using Application.Operation.Features.Employee.Exams.Queries;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Extensions;
using Tawtheef.Domain.Entities.Exams;
using Tawtheef.Domain.Entities.Lookups;

namespace Application.Operation.Features.Employee.Exams.Handlers.Queries;

public sealed class ListExamResultReportsQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<ListExamResultReportsQuery, IResult<PaginatedResult<ExamResultReportListItemDto>>>
{
    public async Task<IResult<PaginatedResult<ExamResultReportListItemDto>>> Handle(ListExamResultReportsQuery request,
        CancellationToken cancellationToken)
    {
        var search = request.Search?.Trim();

        var isArabic = string.Equals(request.Language, "ar", StringComparison.OrdinalIgnoreCase);

        var reports = unitOfWork.Context.Set<ExamResultReport>().AsNoTracking().AsQueryable();

        reports = reports
            .WhereIf(!string.IsNullOrWhiteSpace(search),
                report => EF.Functions.Like(report.Exam!.ExamNo, $"%{search}%")
                          || EF.Functions.Like(report.Exam.TitleAr, $"%{search}%")
                          || (report.Exam.TitleEn != null && EF.Functions.Like(report.Exam.TitleEn, $"%{search}%"))
                          || EF.Functions.Like(report.Exam.Job!.JobTitle!.JobNameAr, $"%{search}%")
                          || EF.Functions.Like(report.Exam.Job.JobTitle.JobNameEn, $"%{search}%"))
            .WhereIf(request.JobId.HasValue, report => report.Exam!.JobId == request.JobId!.Value)
            .WhereIf(request.StatusId.HasValue, report => report.StatusId == request.StatusId!.Value);

        if (request.CreatedFrom is { } createdFrom)
        {
            var start = createdFrom.ToDateTime(TimeOnly.MinValue);

            reports = reports.Where(x => x.CreatedDate >= start);
        }

        if (request.CreatedTo is { } createdTo && createdTo < DateOnly.MaxValue)
        {
            var endExclusive = createdTo.AddDays(1).ToDateTime(TimeOnly.MinValue);

            reports = reports.Where(x => x.CreatedDate < endExclusive);
        }

        var page = await reports.OrderByDescending(x => x.CreatedDate)
            .ThenBy(x => x.Id)
            .Select(report => new ExamResultReportListItemDto
            {
                Id = report.Id,
                ExamId = report.ExamId,
                ExamNo = report.Exam!.ExamNo,
                ExamTitle = isArabic ? report.Exam.TitleAr : report.Exam.TitleEn ?? report.Exam.TitleAr,
                JobId = report.Exam.JobId,
                JobTitle = isArabic ? report.Exam.Job!.JobTitle!.JobNameAr : report.Exam.Job!.JobTitle!.JobNameEn,
                Status =
                    new DropdownOptions
                    {
                        Id = report.StatusId,
                        BackendName = report.Status!.BackendName,
                        Name = isArabic ? report.Status.NameAr : report.Status.NameEn
                    },
                CandidateCount = report.Candidates.Count,
                QualifiedForInterviewCount =
                    report.Candidates.Count(x =>
                        x.ResultStatusId == ExamResultCandidateResultStatusIds.QualifiedForInterview),
                NotQualifiedForInterviewCount =
                    report.Candidates.Count(x =>
                        x.ResultStatusId == ExamResultCandidateResultStatusIds.NotQualifiedForInterview),
                NoShowCount =
                    report.Candidates.Count(x => x.ResultStatusId == ExamResultCandidateResultStatusIds.NoShow),
                NotCompletedCount =
                    report.Candidates.Count(x =>
                        x.ResultStatusId == ExamResultCandidateResultStatusIds.NotCompleted),
                RequiresActionCount =
                    report.Candidates.Count(x =>
                        x.ResultStatusId == ExamResultCandidateResultStatusIds.RequiresAction),
                ApprovedAt = report.ApprovedAt.AsUtcOffset(),
                CreatedDate = report.CreatedDate.AsUtcOffset(),
                LastUpdated = report.UpdatedDate.AsUtcOffset() ?? report.CreatedDate.AsUtcOffset()
            })
            .ToPaginatedListAsync(
                request with
                {
                    PageNumber = Math.Max(1, request.PageNumber),
                    PageSize = Math.Clamp(request.PageSize, 1, 50),
                    SortBy = null
                }, cancellationToken);

        return Result.Ok(page);
    }
}
