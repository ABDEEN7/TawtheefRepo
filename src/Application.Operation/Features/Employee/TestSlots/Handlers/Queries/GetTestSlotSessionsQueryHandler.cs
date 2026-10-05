using Application.Operation.Features.Employee.TestSlots.DTOs;
using Application.Operation.Features.Employee.TestSlots.Queries;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Exams;
using Tawtheef.Domain.Entities.Lookups;

namespace Application.Operation.Features.Employee.TestSlots.Handlers.Queries;

public sealed class GetTestSlotSessionsQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetTestSlotSessionsQuery, IResult<List<TestSlotSessionDto>>>
{
    public async Task<IResult<List<TestSlotSessionDto>>> Handle(GetTestSlotSessionsQuery request, CancellationToken ct)
    {
        var isArabic = string.Equals(request.Language, "ar", StringComparison.OrdinalIgnoreCase);
        var sessions = await unitOfWork.Context.Set<TestSession>().AsNoTracking()
            .Where(s => s.TestSlotId == request.TestSlotId &&
                (s.StatusId == TestSessionStatusIds.Approved || s.StatusId == TestSessionStatusIds.Ready))
            .OrderBy(s => s.SessionNo)
            .Select(s => new TestSlotSessionDto(
                s.Id,
                s.SessionNo,
                isArabic ? s.Exam!.Job!.JobTitle!.JobNameAr : s.Exam!.Job!.JobTitle!.JobNameEn,
                isArabic ? s.Exam!.TitleAr : (s.Exam!.TitleEn ?? s.Exam!.TitleAr),
                s.Exam!.ExamNo,
                unitOfWork.Context.Set<TestSessionCandidate>().Count(c => c.TestSessionId == s.Id),
                s.StartTime,
                s.EndTime,
                s.GenderFilter,
                s.NationalityFilter,
                isArabic ? s.Status!.NameAr : s.Status!.NameEn))
            .ToListAsync(ct);
        return Result.Ok(sessions);
    }
}
