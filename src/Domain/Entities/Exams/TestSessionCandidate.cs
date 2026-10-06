using System.ComponentModel.DataAnnotations.Schema;
using FluentResults;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment;

namespace Tawtheef.Domain.Entities.Exams;

[Table(nameof(TestSessionCandidate), Schema = Schemas.Hr)]
public class TestSessionCandidate : EventEntity
{
    public Guid TestSessionId { get; set; }
    public Guid InvitationId { get; set; }
    public Guid AttendanceStatusId { get; set; }
    public Guid IdentityVerificationStatusId { get; set; }
    public string? IdentityVerificationNotes { get; set; }
    public DateTime? AuthorizedAt { get; set; }
    public Guid StatusId { get; set; }
    public string? RescheduleReason { get; set; }
    public Guid? RescheduledFromTestSessionCandidateId { get; set; }

    public TestSession? TestSession { get; set; }
    public TestSessionCandidate? RescheduledFromTestSessionCandidate { get; set; }
    public Invitation? Invitation { get; set; }
    public TestSessionCandidateAttendanceStatus? AttendanceStatus { get; set; }
    public TestSessionCandidateIdentityVerificationStatus? IdentityVerificationStatus { get; set; }
    public TestSessionCandidateStatus? Status { get; set; }
    public TestAttempt? TestAttempt { get; set; }

    public void MarkExamStarted()
    {
        StatusId = TestSessionCandidateStatusIds.Started;
    }

    public void MarkExamCompleted()
    {
        StatusId = TestSessionCandidateStatusIds.Completed;
    }

    public Result MarkRescheduled(string reason)
    {
        if (StatusId != TestSessionCandidateStatusIds.Assigned &&
            StatusId != TestSessionCandidateStatusIds.Authorized &&
            StatusId != TestSessionCandidateStatusIds.Started &&
            StatusId != TestSessionCandidateStatusIds.Completed)
        {
            return Result.Fail(new Error(ErrorsCodes.TestSessionCandidateCannotBeRescheduled));
        }

        if (string.IsNullOrWhiteSpace(reason))
            return Result.Fail(new Error(ErrorsCodes.TestSessionCandidateRescheduleReasonRequired));

        StatusId = TestSessionCandidateStatusIds.Rescheduled;
        RescheduleReason = reason.Trim();
        return Result.Ok();
    }
    
    public Result MarkPresent()
    {
        if (StatusId == TestSessionCandidateStatusIds.Cancelled ||
            StatusId == TestSessionCandidateStatusIds.Rescheduled ||
            StatusId == TestSessionCandidateStatusIds.Completed)
            return Result.Fail(ErrorsCodes.TestSlotCandidateHallStateInvalid);
        if (AttendanceStatusId == TestSessionCandidateAttendanceStatusIds.Present)
            return Result.Ok();
        if (AttendanceStatusId != TestSessionCandidateAttendanceStatusIds.Pending)
            return Result.Fail(ErrorsCodes.TestSlotCandidateHallStateInvalid);
        AttendanceStatusId = TestSessionCandidateAttendanceStatusIds.Present;
        return Result.Ok();
    }
    public Result MarkNoShow()
    {
        if (StatusId != TestSessionCandidateStatusIds.Assigned ||
            AttendanceStatusId !=
            TestSessionCandidateAttendanceStatusIds.Pending ||
            AuthorizedAt is not null)
        {
            return Result.Fail(
                ErrorsCodes.TestSlotCandidateHallStateInvalid);
        }

        AttendanceStatusId =
            TestSessionCandidateAttendanceStatusIds.NoShow;

        return Result.Ok();
    }
    public Result VerifyIdentity(bool matched, string? notes)
    {
        if (AttendanceStatusId != TestSessionCandidateAttendanceStatusIds.Present ||
            IdentityVerificationStatusId != TestSessionCandidateIdentityVerificationStatusIds.Pending ||
            StatusId != TestSessionCandidateStatusIds.Assigned || notes?.Trim().Length > 1000)
            return Result.Fail(ErrorsCodes.TestSlotCandidateHallStateInvalid);
        if (!matched && string.IsNullOrWhiteSpace(notes))
            return Result.Fail(ErrorsCodes.TestSlotCandidateIdentityNotesRequired);
        IdentityVerificationStatusId = matched
            ? TestSessionCandidateIdentityVerificationStatusIds.Matched
            : TestSessionCandidateIdentityVerificationStatusIds.NotMatched;
        IdentityVerificationNotes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
        return Result.Ok();
    }

    public Result AuthorizeEntry(DateTime authorizedAtUtc)
    {
        if (StatusId != TestSessionCandidateStatusIds.Assigned ||
            AttendanceStatusId != TestSessionCandidateAttendanceStatusIds.Present ||
            IdentityVerificationStatusId != TestSessionCandidateIdentityVerificationStatusIds.Matched ||
            AuthorizedAt is not null)
            return Result.Fail(ErrorsCodes.TestSlotCandidateHallStateInvalid);
        StatusId = TestSessionCandidateStatusIds.Authorized;
        AuthorizedAt = authorizedAtUtc;
        return Result.Ok();
    }
}
