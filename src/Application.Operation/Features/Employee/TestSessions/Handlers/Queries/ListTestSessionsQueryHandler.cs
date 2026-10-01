using Application.Operation.Features.Employee.TestSessions.DTOs;
using Application.Operation.Features.Employee.TestSessions.Queries;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Extensions;
using Tawtheef.Domain.Entities.Exams;
using Tawtheef.Domain.Entities.Lookups;

namespace Application.Operation.Features.Employee.TestSessions.Handlers.Queries;

public sealed class ListTestSessionsQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<ListTestSessionsQuery, IResult<PaginatedResult<TestSessionListItemDto>>>
{
    public async Task<IResult<PaginatedResult<TestSessionListItemDto>>> Handle(
        ListTestSessionsQuery request, CancellationToken cancellationToken)
    {
        var isArabic = string.Equals(request.Language, "ar", StringComparison.OrdinalIgnoreCase);
        var search = request.SearchText?.Trim();
        var candidates = unitOfWork.GetEntityRepository<TestSessionCandidate>().DbSet.AsNoTracking();
        var staff = unitOfWork.GetEntityRepository<TestSlotStaff>().DbSet.AsNoTracking();
        var sessions = unitOfWork.GetEntityRepository<TestSession>().DbSet.AsNoTracking()
            .WhereIf(!string.IsNullOrWhiteSpace(search), session =>
                EF.Functions.Like(session.SessionNo, $"%{search}%") ||
                EF.Functions.Like(session.Exam!.TitleAr, $"%{search}%") ||
                EF.Functions.Like(session.Exam.TitleEn!, $"%{search}%") ||
                EF.Functions.Like(session.Exam.Job!.JobTitle!.JobNameAr, $"%{search}%") ||
                EF.Functions.Like(session.Exam.Job.JobTitle.JobNameEn, $"%{search}%"))
            .WhereIf(request.ExamId.HasValue, session => session.ExamId == request.ExamId)
            .WhereIf(request.JobId.HasValue, session => session.Exam!.Job!.JobTitleId == request.JobId)
            .WhereIf(request.RoomId.HasValue, session => session.TestSlot!.RoomId == request.RoomId)
            .WhereIf(request.FromDate.HasValue, session => request.FromDate != null && session.TestSlot!.SlotDate >= request.FromDate.Value)
            .WhereIf(request.ToDate.HasValue, session => request.ToDate != null && session.TestSlot!.SlotDate <= request.ToDate.Value)
            .WhereIf(request.PeriodId.HasValue, session => session.TestSlotId == request.PeriodId)
            .WhereIf(request.StatusId.HasValue, session => session.StatusId == request.StatusId)
            .WhereIf(request.NationalityId.HasValue,
                session => candidates.Any(candidate =>
                    candidate.TestSessionId == session.Id && candidate.Invitation!.Applicant!.Profile!.NationalityId ==
                    request.NationalityId))
            .WhereIf(request.GenderId.HasValue,
                session => candidates.Any(candidate =>
                    candidate.TestSessionId == session.Id &&
                    candidate.Invitation!.Applicant!.Profile!.GenderId == request.GenderId));

        var page = await sessions.OrderByDescending(session => session.TestSlot != null)
            .ThenByDescending(session => session.TestSlot != null
                ? (DateOnly?)session.TestSlot.SlotDate
                : null)
            .ThenBy(session => session.TestSlot != null ? (TimeOnly?)session.TestSlot.StartTime : null)
            .ThenBy(session => session.SessionNo)
            .Select(session => new TestSessionListItemDto
            {
                Id = session.Id,
                SessionNo = session.SessionNo,
                ExamId = session.ExamId,
                ExamName = isArabic ? session.Exam!.TitleAr : session.Exam!.TitleEn ?? session.Exam.TitleAr,
                JobId = session.Exam!.JobId,
                JobTitle = isArabic ? session.Exam.Job!.JobTitle!.JobNameAr : session.Exam.Job!.JobTitle!.JobNameEn,
                SessionDate = session.TestSlot == null ? null : session.TestSlot.SlotDate,
                PeriodId = session.TestSlotId,
                Period = session.TestSlot == null
                    ? null
                    : isArabic ? session.TestSlot.TitleAr : session.TestSlot.TitleEn ?? session.TestSlot.TitleAr,
                RoomId = session.TestSlot == null ? null : session.TestSlot.RoomId,
                RoomName = session.TestSlot == null || session.TestSlot.Room == null
                    ? null
                    : isArabic
                        ? session.TestSlot.Room.NameAr
                        : session.TestSlot.Room.NameEn ?? session.TestSlot.Room.NameAr,
                CandidateCount = candidates.Count(candidate => candidate.TestSessionId == session.Id),
                RoomHeadName =
                    staff.Where(member =>
                            member.TestSlotId == session.TestSlotId && member.IsActive &&
                            member.RoleId == TestSlotStaffRoleIds.HallSupervisor).OrderBy(member => member.Id)
                        .Select(member => isArabic ? member.StaffUser!.FullNameAr : member.StaffUser!.FullNameEn)
                        .FirstOrDefault(),
                Status = new DropdownOptions
                {
                    Id = session.StatusId,
                    BackendName = session.Status!.BackendName,
                    Name = isArabic ? session.Status.NameAr : session.Status.NameEn
                }
            }).ToPaginatedListAsync(
                request with
                {
                    PageNumber = Math.Max(1, request.PageNumber),
                    PageSize = Math.Clamp(request.PageSize, 1, 50),
                    SortBy = null
                }, cancellationToken);

        return Result.Ok(new PaginatedResult<TestSessionListItemDto>(page.Items, page.Metadata.TotalCount,
            Math.Max(1, request.PageNumber), Math.Clamp(request.PageSize, 1, 50)));
    }
}
