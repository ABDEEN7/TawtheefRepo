using Application.Operation.Features.Employee.TestSlots.DTOs;
using Application.Operation.Features.Employee.TestSlots.Queries;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Exams;

namespace Application.Operation.Features.Employee.TestSlots.Handlers.Queries;

public sealed class GetTestSlotSessionsQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetTestSlotSessionsQuery, IResult<List<TestSlotSessionDto>>>
{
    public async Task<IResult<List<TestSlotSessionDto>>> Handle(GetTestSlotSessionsQuery request, CancellationToken ct)
    {
        var isArabic = string.Equals(request.Language, "ar", StringComparison.OrdinalIgnoreCase);
        var sessions = await unitOfWork.Context.Set<TestSession>().AsNoTracking().Where(s => s.TestSlotId == request.TestSlotId)
            .OrderBy(s => s.SessionNo).Select(s => new TestSlotSessionDto(s.Id, s.SessionNo, s.Exam!.ExamNo,
                isArabic ? s.Status!.NameAr : s.Status!.NameEn,
                unitOfWork.Context.Set<TestSessionCandidate>().Count(c => c.TestSessionId == s.Id))).ToListAsync(ct);
        return Result.Ok(sessions);
    }
}
