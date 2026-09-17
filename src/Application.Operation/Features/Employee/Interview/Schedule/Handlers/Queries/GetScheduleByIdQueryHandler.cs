using Application.Operation.Features.Employee.Interview.Schedule.DTOs;
using Application.Operation.Features.Employee.Interview.Schedule.Queries;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.Schedule.Handlers.Queries;

public sealed class GetScheduleByIdQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetScheduleByIdQuery, IResult<ScheduleDto>>
{
    public async Task<IResult<ScheduleDto>> Handle(GetScheduleByIdQuery request, CancellationToken cancellationToken)
    {
        var schedule = await unitOfWork.GetEntityRepository<InterviewSchedule>().DbSet
            .AsNoTracking()
            .Include(s => s.Job).ThenInclude(j => j!.JobTitle)
            .Include(s => s.InterviewTemplate)
            .Include(s => s.Appointments).ThenInclude(a => a.Invitation).ThenInclude(i => i!.Applicant)
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
        if (schedule is null)
            return Result.Fail<ScheduleDto>(new Error(ErrorsCodes.InterviewScheduleNotFound));

        var committee = await unitOfWork.GetEntityRepository<InterviewCommittee>().DbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.JobId == schedule.JobId && c.IsActive, cancellationToken);

        var appointments = schedule.Appointments
            .OrderBy(a => a.StartAt)
            .Select(a => new AppointmentDto(
                a.Id,
                a.InvitationId,
                a.Invitation?.Applicant?.FullNameAr,
                a.Invitation?.Applicant?.FullNameEn,
                a.InterviewCommitteeId,
                a.InterviewType,
                a.RoomId,
                a.RemoteMeetingUrl,
                a.RemoteMeetingInstructions,
                a.StartAt,
                a.EndAt,
                a.Status,
                a.AttendanceStatus,
                a.ActualStartAt,
                a.ActualEndAt,
                a.ClosedAt,
                a.RescheduledFromAppointmentId,
                a.RescheduleReason,
                a.CancellationReason,
                a.InvitationSentAt,
                a.LastReminderSentAt,
                a.ReminderCount))
            .ToList();

        var dto = new ScheduleDto(
            schedule.Id,
            schedule.JobId,
            schedule.Job?.JobTitle?.JobNameAr,
            schedule.Job?.JobTitle?.JobNameEn,
            schedule.InterviewTemplateId,
            schedule.InterviewTemplate!.TitleAr,
            schedule.InterviewTemplate.TitleEn,
            committee?.Id,
            committee?.NameAr,
            committee?.NameEn,
            schedule.TitleAr,
            schedule.TitleEn,
            schedule.DefaultInterviewType,
            schedule.DefaultDurationMinutes,
            schedule.DefaultBufferMinutes,
            schedule.Status,
            schedule.ApprovedById,
            schedule.ApprovedAt,
            schedule.DecisionNotes,
            appointments);

        return Result.Ok(dto);
    }
}
