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

namespace Application.Operation.Features.Employee.Exams.Handlers.Queries;

public sealed class ListExamsQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<ListExamsQuery, IResult<PaginatedResult<ExamListItemDto>>>
{
    public async Task<IResult<PaginatedResult<ExamListItemDto>>> Handle(
        ListExamsQuery request, CancellationToken cancellationToken)
    {
        var search = request.Search?.Trim();
        var isArabic = string.Equals(request.Language, "ar", StringComparison.OrdinalIgnoreCase);
        var exams = unitOfWork.GetEntityRepository<Exam>().DbSet.AsNoTracking()
            .WhereIf(!string.IsNullOrWhiteSpace(search),
                exam => EF.Functions.Like(exam.ExamNo, $"%{search}%") ||
                        EF.Functions.Like(exam.Job!.JobTitle!.JobNameAr, $"%{search}%") ||
                        EF.Functions.Like(exam.Job!.JobTitle!.JobNameEn, $"%{search}%"))
            .WhereIf(request.StatusId.HasValue, exam => exam.StatusId == request.StatusId)
            .WhereIf(request.SpecializationId.HasValue, exam =>
                exam.Job!.MajorId == request.SpecializationId ||
                exam.Job.SubMajorId == request.SpecializationId ||
                exam.Job.JobSpecializations.Any(s => s.MajorId == request.SpecializationId ||
                                                      s.SubMajorId == request.SpecializationId));

        if (request.CreatedFrom is { } createdFrom)
        {
            var start = createdFrom.ToDateTime(TimeOnly.MinValue);
            exams = exams.Where(exam => exam.CreatedDate >= start);
        }

        if (request.CreatedTo is { } createdTo && createdTo < DateOnly.MaxValue)
        {
            var endExclusive = createdTo.AddDays(1).ToDateTime(TimeOnly.MinValue);
            exams = exams.Where(exam => exam.CreatedDate < endExclusive);
        }

        var parts = unitOfWork.GetEntityRepository<ExamPart>().DbSet.AsNoTracking();
        var page = await exams
            .OrderByDescending(exam => exam.CreatedDate)
            .ThenBy(exam => exam.Id)
            .Select(exam => new
            {
                exam.Id,
                exam.ExamNo,
                JobTitle = isArabic ? exam.Job!.JobTitle!.JobNameAr : exam.Job!.JobTitle!.JobNameEn,
                Major = exam.Job!.Major == null ? null :
                    (isArabic ? exam.Job.Major.NameAr : exam.Job.Major.NameEn),
                SubMajor = exam.Job.SubMajor == null ? null :
                    (isArabic ? exam.Job.SubMajor.NameAr : exam.Job.SubMajor.NameEn),
                AdditionalSpecializations = exam.Job.JobSpecializations
                    .OrderBy(s => s.Id)
                    .Select(s => (isArabic ? s.Major!.NameAr : s.Major!.NameEn) + " / " +
                                 (isArabic ? s.SubMajor!.NameAr : s.SubMajor!.NameEn)).ToList(),
                exam.TotalQuestions,
                TotalDurationMinutes = parts.Where(part => part.ExamId == exam.Id)
                    .Sum(part => (int?)part.DurationMinutes) ?? 0,
                Status = new DropdownOptions
                {
                    Id = exam.StatusId,
                    BackendName = exam.Status!.BackendName,
                    Name = isArabic ? exam.Status.NameAr : exam.Status.NameEn
                },
                exam.CreatedDate,
                LastUpdated = exam.UpdatedDate ?? exam.CreatedDate
            })
            .ToPaginatedListAsync(request with
            {
                PageNumber = Math.Max(1, request.PageNumber),
                PageSize = Math.Clamp(request.PageSize, 1, 50),
                SortBy = null
            }, cancellationToken);

        var items = page.Items.Select(exam =>
        {
            var primary = string.Join(" / ", new[] { exam.Major, exam.SubMajor }
                .Where(name => !string.IsNullOrWhiteSpace(name)));
            var specializations = exam.AdditionalSpecializations;
            if (!string.IsNullOrWhiteSpace(primary))
                specializations.Insert(0, primary);

            return new ExamListItemDto
            {
                Id = exam.Id,
                ExamNo = exam.ExamNo,
                JobTitle = exam.JobTitle,
                Specializations = specializations.Distinct().ToList(),
                TotalQuestions = exam.TotalQuestions,
                TotalDurationMinutes = exam.TotalDurationMinutes,
                Status = exam.Status,
                CreatedDate = exam.CreatedDate,
                LastUpdated = exam.LastUpdated
            };
        }).ToList();

        return Result.Ok(new PaginatedResult<ExamListItemDto>(items,
            page.Metadata.TotalCount, Math.Max(1, request.PageNumber), Math.Clamp(request.PageSize, 1, 50)));
    }
}
