using System.Security.Cryptography;
using Application.Recruitment.Features.CandidateExams.DTOs;
using Application.Recruitment.Features.CandidateExams.Models;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Exams;
using Tawtheef.Domain.Entities.Lookups;

namespace Application.Recruitment.Features.CandidateExams.Services;

public sealed class CandidateExamEntryValidationService(
    IUnitOfWork unitOfWork,
    IAccessCodeProtector accessCodeProtector,
    TimeProvider timeProvider)
{
    public Task<IResult<CandidateExamEntryValidationResult>> ValidateInitialEntryAsync(Guid userId,
        Guid testSessionCandidateId, string accessCode, CancellationToken cancellationToken) =>
        ValidateAsync(userId, testSessionCandidateId, accessCode, false, cancellationToken);

    public Task<IResult<CandidateExamEntryValidationResult>> ValidateForResumeAsync(Guid userId,
        Guid testSessionCandidateId,
        string accessCode,
        CancellationToken cancellationToken) =>
        ValidateAsync(userId, testSessionCandidateId, accessCode, true, cancellationToken);

    private async Task<IResult<CandidateExamEntryValidationResult>> ValidateAsync(Guid userId,
        Guid testSessionCandidateId, string accessCode, bool allowStarted, CancellationToken cancellationToken)
    {
        var a = await unitOfWork.Context
            .Set<TestSessionCandidate>()
            .AsNoTracking()
            .Where(x =>
                x.Id == testSessionCandidateId &&
                x.Invitation!.ApplicantId == userId)
            .Select(x => new CandidateExamAssignmentValidationData(
                x.Id,
                x.TestSessionId,
                x.TestSession!.ExamId,
                x.Invitation!.IsAccepted,
                x.Invitation.AcceptedAt,
                x.Invitation.InvitationStatusId,
                x.TestSession.Exam!.StatusId,
                x.StatusId,
                x.TestSession.TestSlot!.StatusId,
                x.TestSession.StatusId,
                x.AttendanceStatusId,
                x.IdentityVerificationStatusId,
                x.TestSession.TestSlot.SlotDate,
                x.TestSession.TestSlot.EndTime,
                x.TestSession.TestSlot.AccessCodeHash))
            .SingleOrDefaultAsync(cancellationToken);

        if (a is null)
            return Failure(ErrorsCodes.CandidateExamAssignmentNotFound, StatusCodes.Status404NotFound);
        if (!a.InvitationIsAccepted || a.InvitationAcceptedAt is null ||
            a.InvitationStatusId != InvitationStatusIds.ExamEligible)
            return Failure(ErrorsCodes.CandidateExamInvitationNotEligible);
        if (a.ExamStatusId != ExamStatusIds.Approved)
            return Failure(ErrorsCodes.CandidateExamNotApproved);
        if (a.CandidateStatusId == TestSessionCandidateStatusIds.Completed)
            return Failure(ErrorsCodes.CandidateExamAlreadyCompleted, StatusCodes.Status409Conflict);
        if (a.CandidateStatusId == TestSessionCandidateStatusIds.Cancelled)
            return Failure(ErrorsCodes.CandidateExamCancelled, StatusCodes.Status409Conflict);
        if (a.CandidateStatusId == TestSessionCandidateStatusIds.Rescheduled)
            return Failure(ErrorsCodes.CandidateExamRescheduled, StatusCodes.Status409Conflict);
        if (a.CandidateStatusId == TestSessionCandidateStatusIds.Started && !allowStarted)
            return Failure(ErrorsCodes.CandidateExamAlreadyStarted, StatusCodes.Status409Conflict);
        if (a.CandidateStatusId != TestSessionCandidateStatusIds.Started)
        {
            var localNow = timeProvider.GetLocalNow().DateTime;
            var scheduledEnd = a.SlotDate.ToDateTime(a.SlotEndTime);

            if (localNow >= scheduledEnd)
            { 
                return Failure( ErrorsCodes.CandidateExamEntryWindowClosed,StatusCodes.Status409Conflict);
            }
        }

        if (a.SlotStatusId != TestSlotStatusIds.Started)
            return Failure(ErrorsCodes.CandidateExamSlotNotRunning);
        if (a.SessionStatusId != TestSessionStatusIds.InProgress)
            return Failure(ErrorsCodes.CandidateExamSessionNotRunning);
        if (a.AttendanceStatusId != TestSessionCandidateAttendanceStatusIds.Present)
            return Failure(ErrorsCodes.CandidateExamAttendanceNotPresent);
        if (a.IdentityStatusId != TestSessionCandidateIdentityVerificationStatusIds.Matched)
            return Failure(ErrorsCodes.CandidateExamIdentityNotMatched);
        if (a.CandidateStatusId != TestSessionCandidateStatusIds.Authorized &&
            !(allowStarted && a.CandidateStatusId == TestSessionCandidateStatusIds.Started))
            return Failure(ErrorsCodes.CandidateExamNotAuthorized);

        string storedAccessCode;
        try
        {
            storedAccessCode = accessCodeProtector.Unprotect(a.StoredAccessCode);
        }
        catch (Exception exception) when (exception is CryptographicException or ArgumentException)
        {
            return Failure(ErrorsCodes.CandidateExamInvalidAccessCode);
        }

        if (!string.Equals(storedAccessCode, accessCode, StringComparison.Ordinal))
        {
            return Failure(ErrorsCodes.CandidateExamInvalidAccessCode);
        }


        return Result.Ok(new CandidateExamEntryValidationResult(a.Id, a.TestSessionId, a.ExamId, a.CandidateStatusId));
    }

    private static IResult<CandidateExamEntryValidationResult> Failure(string code, int statusCode = 400) =>
        Result.Fail<CandidateExamEntryValidationResult>(new Error(code).WithMetadata("Code", code)
            .WithMetadata("StatusCode", statusCode));
}
