using Application.Operation.Features.Employee.Interview.Schedule.DTOs;
using Application.Operation.Features.Employee.Interview.Schedule.Queries;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Extensions;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.Schedule.Handlers.Queries;

public sealed class ListScheduleAppointmentsQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<ListScheduleAppointmentsQuery, IResult<List<AppointmentDto>>>
{
    public async Task<IResult<List<AppointmentDto>>> Handle(ListScheduleAppointmentsQuery request, CancellationToken cancellationToken)
    {
        var appointments = await unitOfWork.GetEntityRepository<InterviewAppointment>().DbSet
            .AsNoTracking()
            .Where(a => a.InterviewScheduleId == request.InterviewScheduleId)
            .OrderBy(a => a.StartAt)
            .Select(a => new AppointmentDto(
                a.Id,
                a.InvitationId,
                a.Invitation != null ? a.Invitation.Applicant!.FullNameAr : null,
                a.Invitation != null ? a.Invitation.Applicant!.FullNameEn : null,
                a.Invitation != null ? a.Invitation.Applicant!.Profile!.NationalNumber : null,
                a.InterviewCommitteeId,
                a.InterviewType,
                a.RoomId,
                a.Room != null ? a.Room.NameAr : null,
                a.Room != null ? a.Room.NameEn : null,
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
            .ToListAsync(cancellationToken);

        return Result.Ok(appointments);
    }
}
