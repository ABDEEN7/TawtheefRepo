using Application.Operation.Features.Employee.TestSlots.DTOs;
using Application.Operation.Features.Employee.TestSlots.Queries;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Extensions;
using Tawtheef.Domain.Entities.Exams;

namespace Application.Operation.Features.Employee.TestSlots.Handlers.Queries;

public sealed class GetTestSlotCandidatesQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetTestSlotCandidatesQuery, IResult<PaginatedResult<TestSlotCandidateListItemDto>>>
{
    public async Task<IResult<PaginatedResult<TestSlotCandidateListItemDto>>> Handle(
        GetTestSlotCandidatesQuery request,
        CancellationToken ct)
    {
        var isArabic = string.Equals(request.Language, "ar", StringComparison.OrdinalIgnoreCase);
        var candidates = unitOfWork.Context.Set<TestSessionCandidate>().AsNoTracking()
            .Where(candidate => candidate.TestSession!.TestSlotId == request.TestSlotId)
            .WhereIf(request.SessionId.HasValue, candidate => candidate.TestSessionId == request.SessionId)
            .WhereIf(request.AttendanceStatusId.HasValue,
                candidate => candidate.AttendanceStatusId == request.AttendanceStatusId)
            .WhereIf(!string.IsNullOrWhiteSpace(request.Search), candidate =>
                EF.Functions.Like(isArabic ? candidate.Invitation!.Applicant!.FullNameAr : candidate.Invitation!.Applicant!.FullNameEn,
                    $"%{request.Search!.Trim()}%") ||
                (candidate.Invitation!.Applicant!.Profile!.NationalNumber != null &&
                 EF.Functions.Like(candidate.Invitation.Applicant.Profile.NationalNumber, $"%{request.Search.Trim()}%")));

        var totalCount = await candidates.CountAsync(ct);
        var items = await candidates.OrderBy(candidate => candidate.TestSession!.SessionNo)
            .ThenBy(candidate => candidate.Id)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(candidate => new TestSlotCandidateListItemDto(
                candidate.Id,
                isArabic ? candidate.Invitation!.Applicant!.FullNameAr : candidate.Invitation!.Applicant!.FullNameEn,
                candidate.Invitation!.Applicant!.Profile!.NationalNumber,
                isArabic ? candidate.TestSession!.Exam!.TitleAr : candidate.TestSession!.Exam!.TitleEn ?? candidate.TestSession.Exam.TitleAr,
                isArabic ? candidate.TestSession.Exam!.Job!.JobTitle!.JobNameAr : candidate.TestSession.Exam!.Job!.JobTitle!.JobNameEn,
                candidate.TestSession.SessionNo,
                candidate.TestSession.Exam.ExamNo,
                isArabic ? candidate.AttendanceStatus!.NameAr : candidate.AttendanceStatus!.NameEn,
                isArabic ? candidate.Status!.NameAr : candidate.Status!.NameEn,
                null)).ToListAsync(ct);

        return Result.Ok(new PaginatedResult<TestSlotCandidateListItemDto>(
            items, totalCount, request.PageNumber, request.PageSize));
    }
}
