using Application.Operation.Features.Employee.Interview.Schedule.DTOs;
using Application.Operation.Features.Employee.Interview.Schedule.Queries;
using Application.Operation.Features.Employee.Interview.Schedule.Services;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Extensions;
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
            .Include(s => s.Appointments).ThenInclude(a => a.Invitation).ThenInclude(i => i!.Applicant).ThenInclude(u => u!.Profile)
            .Include(s => s.Appointments).ThenInclude(a => a.Room)
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
                a.Invitation?.Applicant?.Profile?.NationalNumber,
                a.InterviewCommitteeId,
                a.InterviewType,
                a.RoomId,
                a.Room?.NameAr,
                a.Room?.NameEn,
                a.RemoteMeetingUrl,
                a.RemoteMeetingInstructions,
                a.StartAt,
                a.EndAt,
                a.Status,
                a.AttendanceStatus,
                a.ActualStartAt.AsUtcOffset(),
                a.ActualEndAt.AsUtcOffset(),
                a.ClosedAt.AsUtcOffset(),
                a.RescheduledFromAppointmentId,
                a.RescheduleReason,
                a.CancellationReason,
                a.InvitationSentAt.AsUtcOffset(),
                a.LastReminderSentAt.AsUtcOffset(),
                a.ReminderCount))
            .ToList();

        // Superseded (rescheduled) and cancelled rows are not part of the live slot set the edit wizard restores.
        var liveAppointments = schedule.Appointments
            .Where(a => a.Status is not (AppointmentStatus.Rescheduled or AppointmentStatus.Cancelled))
            .ToList();
        var roomNames = liveAppointments
            .Where(a => a.Room is not null)
            .GroupBy(a => a.RoomId!.Value)
            .ToDictionary(g => g.Key, g => (g.First().Room!.NameAr, g.First().Room!.NameEn));
        var periods = ScheduleAppointmentPlanner
            .ReconstructPeriods(
                liveAppointments.Select(a => new GeneratedSlotDto(
                    DateOnly.FromDateTime(a.StartAt), a.StartAt, a.EndAt,
                    a.RoomId, a.RemoteMeetingUrl, a.RemoteMeetingInstructions)),
                schedule.DefaultBufferMinutes)
            .Select(p => new SchedulePeriodDto(
                p.Date, p.StartTime, p.EndTime,
                p.RoomId,
                p.RoomId is not null && roomNames.TryGetValue(p.RoomId.Value, out var room) ? room.NameAr : null,
                p.RoomId is not null && roomNames.TryGetValue(p.RoomId.Value, out room) ? room.NameEn : null,
                p.RemoteMeetingUrl,
                p.RemoteMeetingInstructions))
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
            schedule.ApprovedAt.AsUtcOffset(),
            schedule.DecisionNotes,
            appointments,
            periods);

        return Result.Ok(dto);
    }
}
