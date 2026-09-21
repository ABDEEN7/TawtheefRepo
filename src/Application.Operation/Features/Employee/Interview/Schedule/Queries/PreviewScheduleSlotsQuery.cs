using Application.Operation.Features.Employee.Interview.Schedule.DTOs;
using FluentResults;
using MediatR;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.Schedule.Queries;

// Read-only, server-computed preview of the wizard's current periods/distribution (step 2/3) -
// nothing is persisted. ExcludeScheduleId lets the edit/resubmit wizard preview against the
// eligible pool without its own former appointments counting as conflicts.
public sealed record PreviewScheduleSlotsQuery(
    Guid JobId,
    InterviewType InterviewType,
    int DurationMinutes,
    int BufferMinutes,
    List<PeriodInputDto> Periods,
    List<SlotAssignmentDto>? ManualAssignments,
    Guid? ExcludeScheduleId) : IRequest<IResult<SchedulePlanPreviewDto>>;
