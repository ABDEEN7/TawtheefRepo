using Application.Operation.Features.Employee.Interview.Schedule.DTOs;
using FluentResults;
using MediatR;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.Schedule.Commands;

// The whole wizard (job+type -> periods -> distribution -> review) submits as one atomic call -
// not four separate requests (schedule.md). ManualAssignments is optional: omitted/empty means
// fully automatic distribution.
public sealed record CreateScheduleCommand(
    Guid JobId,
    InterviewType InterviewType,
    string TitleAr,
    string? TitleEn,
    int DurationMinutes,
    int BufferMinutes,
    List<PeriodInputDto> Periods,
    List<SlotAssignmentDto>? ManualAssignments) : IRequest<IResult<Guid>>;
