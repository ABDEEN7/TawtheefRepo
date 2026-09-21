using Application.Operation.Features.Employee.Interview.Schedule.DTOs;
using FluentResults;
using MediatR;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.Schedule.Commands;

// Edit/resubmit of a Returned schedule - regenerates the whole appointment list from scratch and
// flips the status back to PendingApproval. Same payload shape as Create + Id.
public sealed record UpdateScheduleCommand(
    Guid Id,
    InterviewType InterviewType,
    string TitleAr,
    string? TitleEn,
    int DurationMinutes,
    int BufferMinutes,
    List<PeriodInputDto> Periods,
    List<SlotAssignmentDto>? ManualAssignments) : IRequest<IResult<Unit>>;
