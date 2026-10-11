using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Exams;
using Tawtheef.Domain.Entities.Interview;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Common.Services.Interview;

/// <summary>Who a candidate-facing interview message goes to, and for which job.</summary>
public sealed record InterviewCandidate(ApplicantUser Applicant, string? JobAr, string? JobEn)
{
    public string Job(string lang) => JobAr is null ? "" : InterviewNotificationText.Pick(lang, JobAr, JobEn);

    /// <summary>The booking's candidate, or null when there is nobody to tell (empty slot, blocked / deleted user).</summary>
    public static async Task<InterviewCandidate?> LoadAsync(IUnitOfWork unitOfWork, Guid? invitationId, CancellationToken ct)
    {
        if (invitationId is null) return null;

        var invitation = await unitOfWork.GetEntityRepository<Invitation>().DbSet
            .AsNoTracking()
            .Include(i => i.Applicant)
            .FirstOrDefaultAsync(i => i.Id == invitationId, ct);
        var applicant = invitation?.Applicant;
        if (applicant is null || applicant.IsBlocked || applicant.IsDeleted) return null;

        var (jobAr, jobEn) = await InterviewWorkflowItems.JobTitleAsync(unitOfWork, invitation!.JobId, ct);
        return new InterviewCandidate(applicant, jobAr, jobEn);
    }
}

/// <summary>When and where one booking takes place, in the recipient's language and Qatar time.</summary>
public sealed record InterviewAppointmentSlot(InterviewAppointment Appointment, Room? Room)
{
    public bool IsRemote => Appointment.InterviewType == InterviewType.Remote;

    public string Day(string lang) => InterviewNotificationText.QatarDayName(Appointment.StartAt, lang);
    public string Date => InterviewNotificationText.QatarDate(Appointment.StartAt);
    public string Start(string lang) => InterviewNotificationText.QatarTime(Appointment.StartAt, lang);
    public string End(string lang) => InterviewNotificationText.QatarTime(Appointment.EndAt, lang);

    /// <summary>In person: "Room, Building" in the recipient's language.</summary>
    public string? Location(string lang)
    {
        if (IsRemote || Room is null) return null;
        var roomName = InterviewNotificationText.Pick(lang, Room.NameAr, Room.NameEn);
        if (Room.Location is null) return roomName;

        var separator = InterviewNotificationText.IsArabic(lang) ? "، " : ", ";
        return roomName + separator + InterviewNotificationText.Pick(lang, Room.Location.NameAr, Room.Location.NameEn);
    }

    public string? LocationMapUrl => IsRemote ? null : NullIfBlank(Room?.Location?.LocationLink);
    public string? MeetingUrl => IsRemote ? NullIfBlank(Appointment.RemoteMeetingUrl) : null;
    public string? Instructions => NullIfBlank(Appointment.RemoteMeetingInstructions);

    public static async Task<InterviewAppointmentSlot> LoadAsync(
        IUnitOfWork unitOfWork, InterviewAppointment appointment, CancellationToken ct)
    {
        var room = appointment.RoomId is null
            ? null
            : await unitOfWork.GetEntityRepository<Room>().DbSet
                .AsNoTracking()
                .Include(r => r.Location)
                .FirstOrDefaultAsync(r => r.Id == appointment.RoomId, ct);
        return new InterviewAppointmentSlot(appointment, room);
    }

    private static string? NullIfBlank(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
