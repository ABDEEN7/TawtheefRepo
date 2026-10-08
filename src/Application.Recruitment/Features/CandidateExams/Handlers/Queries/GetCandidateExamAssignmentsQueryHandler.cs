using Application.Recruitment.Features.CandidateExams.DTOs;
using Application.Recruitment.Features.CandidateExams.Queries;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Common.Models;
using Tawtheef.Application.Extensions;
using Tawtheef.Domain.Entities.Exams;
using Tawtheef.Domain.Entities.Lookups;

namespace Application.Recruitment.Features.CandidateExams.Handlers.Queries;

public sealed class GetCandidateExamAssignmentsQueryHandler(
    IUnitOfWork unitOfWork,
    ILocalizationService localizationService,
    TimeProvider timeProvider)
    : IRequestHandler<GetCandidateExamAssignmentsQuery, IResult<List<CandidateExamAssignmentDto>>>
{
    public async Task<IResult<List<CandidateExamAssignmentDto>>> Handle(GetCandidateExamAssignmentsQuery query,
        CancellationToken cancellationToken)
    {
        var isArabic = localizationService.GetCurrentLanguage() == "ar";
        var localNow = timeProvider.GetLocalNow().DateTime;
        var localDate = DateOnly.FromDateTime(localNow);
        var localTime = TimeOnly.FromDateTime(localNow);

        var assignments = await unitOfWork.GetEntityRepository<TestSessionCandidate>()
            .DbSet.AsNoTracking()
            .Where(candidate => candidate.Invitation!.ApplicantId == query.UserId)
            .OrderBy(candidate => candidate.TestSession!.TestSlot!.SlotDate)
            .ThenBy(candidate => candidate.TestSession!.StartTime)
            .ThenBy(candidate => candidate.TestSession!.SessionNo)
            .Select(candidate => new CandidateExamAssignmentDto
            {
                TestSessionCandidateId = candidate.Id,
                TestSessionId = candidate.TestSessionId,
                ExamId = candidate.TestSession!.ExamId,
                TestAttemptId = candidate.TestAttempt != null ? candidate.TestAttempt.Id : null,
                ExamNo = candidate.TestSession.Exam!.ExamNo,
                ExamTitleAr = candidate.TestSession.Exam.TitleAr,
                ExamTitleEn = candidate.TestSession.Exam.TitleEn,
                JobTitleAr = candidate.Invitation!.Job!.JobTitle!.JobNameAr,
                JobTitleEn = candidate.Invitation.Job.JobTitle.JobNameEn,
                TotalQuestions = candidate.TestSession.Exam.TotalQuestions,
                AllowPreviousQuestion = candidate.TestSession.Exam.AllowPreviousQuestion,
                SessionNo = candidate.TestSession.SessionNo,
                SlotDate = candidate.TestSession.TestSlot!.SlotDate,
                StartTime = candidate.TestSession.StartTime!.Value,
                EndTime = candidate.TestSession.EndTime!.Value,
                RoomNameAr = candidate.TestSession.TestSlot.Room!.NameAr,
                RoomNameEn = candidate.TestSession.TestSlot.Room.NameEn,
                LocationNameAr = candidate.TestSession.TestSlot.Room.Location!.NameAr,
                LocationNameEn = candidate.TestSession.TestSlot.Room.Location.NameEn,
                ExamStatus =
                    new DropdownOptions
                    {
                        Id = candidate.TestSession.Exam.Status!.Id,
                        BackendName = candidate.TestSession.Exam.Status.BackendName,
                        Name =
                            isArabic
                                ? candidate.TestSession.Exam.Status.NameAr
                                : candidate.TestSession.Exam.Status.NameEn,
                        Description =
                            isArabic
                                ? candidate.TestSession.Exam.Status.DescriptionAr
                                : candidate.TestSession.Exam.Status.DescriptionEn
                    },
                SlotStatus =
                    new DropdownOptions
                    {
                        Id = candidate.TestSession.TestSlot.Status!.Id,
                        BackendName = candidate.TestSession.TestSlot.Status.BackendName,
                        Name =
                            isArabic
                                ? candidate.TestSession.TestSlot.Status.NameAr
                                : candidate.TestSession.TestSlot.Status.NameEn,
                        Description =
                            isArabic
                                ? candidate.TestSession.TestSlot.Status.DescriptionAr
                                : candidate.TestSession.TestSlot.Status.DescriptionEn
                    },
                SessionStatus =
                    new DropdownOptions
                    {
                        Id = candidate.TestSession.Status!.Id,
                        BackendName = candidate.TestSession.Status.BackendName,
                        Name =
                            isArabic
                                ? candidate.TestSession.Status.NameAr
                                : candidate.TestSession.Status.NameEn,
                        Description =
                            isArabic
                                ? candidate.TestSession.Status.DescriptionAr
                                : candidate.TestSession.Status.DescriptionEn
                    },
                CandidateStatus =
                    new DropdownOptions
                    {
                        Id = candidate.Status!.Id,
                        BackendName = candidate.Status.BackendName,
                        Name = isArabic ? candidate.Status.NameAr : candidate.Status.NameEn,
                        Description = isArabic ? candidate.Status.DescriptionAr : candidate.Status.DescriptionEn
                    },
                AttendanceStatus =
                    new DropdownOptions
                    {
                        Id = candidate.AttendanceStatus!.Id,
                        BackendName = candidate.AttendanceStatus.BackendName,
                        Name = isArabic ? candidate.AttendanceStatus.NameAr : candidate.AttendanceStatus.NameEn,
                        Description =
                            isArabic
                                ? candidate.AttendanceStatus.DescriptionAr
                                : candidate.AttendanceStatus.DescriptionEn
                    },
                IdentityVerificationStatus =
                    new DropdownOptions
                    {
                        Id = candidate.IdentityVerificationStatus!.Id,
                        BackendName = candidate.IdentityVerificationStatus.BackendName,
                        Name =
                            isArabic
                                ? candidate.IdentityVerificationStatus.NameAr
                                : candidate.IdentityVerificationStatus.NameEn,
                        Description =
                            isArabic
                                ? candidate.IdentityVerificationStatus.DescriptionAr
                                : candidate.IdentityVerificationStatus.DescriptionEn
                    },
                AttemptStatus =
                    candidate.TestAttempt == null
                        ? null
                        : new DropdownOptions
                        {
                            Id = candidate.TestAttempt.Status!.Id,
                            BackendName = candidate.TestAttempt.Status.BackendName,
                            Name =
                                isArabic
                                    ? candidate.TestAttempt.Status.NameAr
                                    : candidate.TestAttempt.Status.NameEn,
                            Description =
                                isArabic
                                    ? candidate.TestAttempt.Status.DescriptionAr
                                    : candidate.TestAttempt.Status.DescriptionEn
                        },
                ResumeAuthorized =
                    candidate.TestAttempt != null
                    && candidate.TestAttempt.StatusId == TestAttemptStatusIds.Interrupted
                    && unitOfWork.Context.Set<TestAttemptInterruption>()
                        .Any(interruption => interruption.TestAttemptId == candidate.TestAttempt.Id
                                             && interruption.StatusId == TestAttemptInterruptionStatusIds.Open
                                             && interruption.ResolutionActionId
                                             == TestAttemptInterruptionResolutionActionIds.Resume
                                             && interruption.ResolvedById != null
                                             && interruption.ResolvedAt == null),
                AuthorizedAt = candidate.AuthorizedAt.AsUtcOffset(),
                EntryState = candidate.StatusId == TestSessionCandidateStatusIds.Completed
                    ?
                    CandidateExamEntryStates.Completed
                    : candidate.StatusId == TestSessionCandidateStatusIds.Cancelled
                      || candidate.TestSession.StatusId == TestSessionStatusIds.Cancelled
                      || candidate.TestSession.Exam.StatusId == ExamStatusIds.Cancelled
                      || candidate.TestSession.Exam.StatusId == ExamStatusIds.Rejected
                        ? CandidateExamEntryStates.Cancelled
                        : candidate.StatusId == TestSessionCandidateStatusIds.Rescheduled
                          || candidate.TestSession.TestSlot.StatusId == TestSlotStatusIds.Rescheduled
                            ? CandidateExamEntryStates.Rescheduled
                            : candidate.StatusId == TestSessionCandidateStatusIds.Started
                                ? CandidateExamEntryStates.Started
                                : candidate.AttendanceStatusId == TestSessionCandidateAttendanceStatusIds.NoShow
                                    ? CandidateExamEntryStates.NoShow
                                    : candidate.TestSession.StatusId == TestSessionStatusIds.Closed
                                      || candidate.TestSession.TestSlot.StatusId == TestSlotStatusIds.Closed
                                        ? CandidateExamEntryStates.Closed
                                        : !candidate.Invitation!.IsAccepted
                                          || candidate.Invitation.AcceptedAt == null
                                          || candidate.Invitation.InvitationStatusId
                                          != InvitationStatusIds.ExamEligible
                                          || candidate.TestSession.Exam.StatusId != ExamStatusIds.Approved
                                            ? CandidateExamEntryStates.NotEligible
                                            : candidate.TestSession.TestSlot.StatusId != TestSlotStatusIds.Started
                                              || candidate.TestSession.StartTime == null
                                              || candidate.TestSession.EndTime == null
                                              || candidate.TestSession.TestSlot.SlotDate != localDate
                                              || candidate.TestSession.StartTime.Value > localTime
                                              || candidate.TestSession.EndTime.Value <= localTime
                                              || (candidate.TestSession.StatusId != TestSessionStatusIds.Approved
                                                  && candidate.TestSession.StatusId != TestSessionStatusIds.Ready
                                                  && candidate.TestSession.StatusId
                                                  != TestSessionStatusIds.InProgress)
                                                ? CandidateExamEntryStates.WaitingForSession
                                                : candidate.AttendanceStatusId
                                                  != TestSessionCandidateAttendanceStatusIds.Present
                                                    ? CandidateExamEntryStates.WaitingForAttendance
                                                    : candidate.IdentityVerificationStatusId
                                                      != TestSessionCandidateIdentityVerificationStatusIds.Matched
                                                        ? CandidateExamEntryStates.WaitingForIdentityVerification
                                                        : candidate.StatusId != TestSessionCandidateStatusIds
                                                            .Authorized
                                                            ? candidate.StatusId == TestSessionCandidateStatusIds
                                                                .Assigned
                                                                ? CandidateExamEntryStates.WaitingForAuthorization
                                                                : CandidateExamEntryStates.Assigned
                                                            : CandidateExamEntryStates.ReadyToEnter
            })
            .ToListAsync(cancellationToken);

        if (assignments.Count == 0)
        {
            return Result.Ok(assignments);
        }

        var examIds = assignments.Select(x => x.ExamId).Distinct().ToArray();

        var examDurations = await unitOfWork.Context.Set<ExamPart>()
            .AsNoTracking()
            .Where(part => examIds.Contains(part.ExamId))
            .GroupBy(part => part.ExamId)
            .Select(group => new { ExamId = group.Key, DurationMinutes = group.Sum(part => part.DurationMinutes) })
            .ToDictionaryAsync(x => x.ExamId, x => x.DurationMinutes, cancellationToken);

        assignments = assignments.Select(assignment =>
            {
                if (assignment.EntryState == CandidateExamEntryStates.Started
                    || !examDurations.TryGetValue(assignment.ExamId, out var durationMinutes) || durationMinutes <= 0)
                {
                    return assignment;
                }

                var scheduledStart = assignment.SlotDate.ToDateTime(assignment.StartTime);

                var scheduledEnd = assignment.SlotDate.ToDateTime(assignment.EndTime);

                var latestAllowedStart = scheduledEnd.AddMinutes(-durationMinutes);

                var isPreEntryState = assignment.EntryState == CandidateExamEntryStates.Assigned
                                      || assignment.EntryState == CandidateExamEntryStates.WaitingForSession
                                      || assignment.EntryState == CandidateExamEntryStates.WaitingForAttendance
                                      || assignment.EntryState
                                      == CandidateExamEntryStates.WaitingForIdentityVerification
                                      || assignment.EntryState == CandidateExamEntryStates.WaitingForAuthorization
                                      || assignment.EntryState == CandidateExamEntryStates.ReadyToEnter;

                if (isPreEntryState && localNow >= scheduledStart && localNow > latestAllowedStart)
                {
                    return assignment with { EntryState = CandidateExamEntryStates.EntryWindowClosed };
                }

                return assignment;
            })
            .ToList();

        return Result.Ok(assignments);
    }
}
