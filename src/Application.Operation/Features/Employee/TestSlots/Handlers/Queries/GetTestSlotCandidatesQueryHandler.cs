using Application.Operation.Features.Employee.TestSlots.DTOs;
using Application.Operation.Features.Employee.TestSlots.Queries;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Extensions;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Exams;
using Tawtheef.Domain.Entities.Lookups;

namespace Application.Operation.Features.Employee.TestSlots.Handlers.Queries;

public sealed class GetTestSlotCandidatesQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    : IRequestHandler<GetTestSlotCandidatesQuery, IResult<PaginatedResult<TestSlotCandidateListItemDto>>>
{
    public async Task<IResult<PaginatedResult<TestSlotCandidateListItemDto>>> Handle(GetTestSlotCandidatesQuery request,
        CancellationToken ct)
    {
        var scope = await TestSlotReadScope.ResolveAsync(unitOfWork, currentUser, ct);

        if (!await TestSlotReadScope.CanAccessAsync(unitOfWork, scope, request.TestSlotId, ct))
        {
            return Result.Fail<PaginatedResult<TestSlotCandidateListItemDto>>(
                new Error(ErrorsCodes.UnauthorizedAction).WithMetadata("StatusCode", StatusCodes.Status403Forbidden));
        }

        var isArabic = string.Equals(request.Language, "ar", StringComparison.OrdinalIgnoreCase);
        var candidates = unitOfWork.Context.Set<TestSessionCandidate>()
            .AsNoTracking()
            .Where(candidate => candidate.TestSession!.TestSlotId == request.TestSlotId)
            .WhereIf(request.SessionId.HasValue, candidate => candidate.TestSessionId == request.SessionId)
            .WhereIf(request.AttendanceStatusId.HasValue,
                candidate => candidate.AttendanceStatusId == request.AttendanceStatusId)
            .WhereIf(!string.IsNullOrWhiteSpace(request.Search),
                candidate =>
                    EF.Functions.Like(
                        isArabic
                            ? candidate.Invitation!.Applicant!.FullNameAr
                            : candidate.Invitation!.Applicant!.FullNameEn, $"%{request.Search!.Trim()}%")
                    || (candidate.Invitation!.Applicant!.Profile!.NationalNumber != null
                        && EF.Functions.Like(candidate.Invitation.Applicant.Profile.NationalNumber,
                            $"%{request.Search.Trim()}%")));

        var totalCount = await candidates.CountAsync(ct);
        var items = await candidates.OrderBy(candidate => candidate.TestSession!.SessionNo)
            .ThenBy(candidate => candidate.Id)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(candidate => new TestSlotCandidateListItemDto(candidate.Id,
                isArabic ? candidate.Invitation!.Applicant!.FullNameAr : candidate.Invitation!.Applicant!.FullNameEn,
                candidate.Invitation!.Applicant!.Profile!.NationalNumber,
                isArabic
                    ? candidate.TestSession!.Exam!.TitleAr
                    : candidate.TestSession!.Exam!.TitleEn ?? candidate.TestSession.Exam.TitleAr,
                isArabic
                    ? candidate.TestSession.Exam!.Job!.JobTitle!.JobNameAr
                    : candidate.TestSession.Exam!.Job!.JobTitle!.JobNameEn, candidate.TestSession.SessionNo,
                candidate.TestSession.Exam.ExamNo,
                isArabic ? candidate.AttendanceStatus!.NameAr : candidate.AttendanceStatus!.NameEn,
                candidate.TestAttempt != null
                    ?
                    isArabic ? candidate.TestAttempt.Status!.NameAr : candidate.TestAttempt.Status!.NameEn
                    : isArabic
                        ? candidate.Status!.NameAr
                        : candidate.Status!.NameEn, candidate.TestAttempt != null ? candidate.TestAttempt.Id : null,
                candidate.TestAttempt != null ? candidate.TestAttempt.Status!.BackendName : null,
                candidate.TestAttempt != null && candidate.TestAttempt.StatusId == TestAttemptStatusIds.Interrupted
                                              && unitOfWork.Context.Set<TestAttemptInterruption>()
                                                  .Any(interruption =>
                                                      interruption.TestAttemptId == candidate.TestAttempt.Id
                                                      && interruption.StatusId == TestAttemptInterruptionStatusIds.Open
                                                      && interruption.ResolutionActionId
                                                      == TestAttemptInterruptionResolutionActionIds.Resume
                                                      && interruption.ResolvedById != null
                                                      && interruption.ResolvedAt == null),
                candidate.TestSession.Exam!.InterruptionPolicyId == ExamInterruptionPolicyIds.ResumeOnly
                || candidate.TestSession.Exam.InterruptionPolicyId == ExamInterruptionPolicyIds.ResumeOrReschedule,
                candidate.TestSession.Exam.InterruptionPolicyId == ExamInterruptionPolicyIds.RescheduleOnly
                || candidate.TestSession.Exam.InterruptionPolicyId == ExamInterruptionPolicyIds.ResumeOrReschedule,
                null, candidate.AttendanceStatus!.BackendName,
                isArabic ? candidate.IdentityVerificationStatus!.NameAr : candidate.IdentityVerificationStatus!.NameEn,
                candidate.IdentityVerificationStatus!.BackendName,
                isArabic ? candidate.Status!.NameAr : candidate.Status!.NameEn, candidate.Status!.BackendName,
                candidate.IdentityVerificationNotes,
                candidate.TestAttempt != null && unitOfWork.Context.Set<TestAttemptInterruption>()
                    .Any(interruption => interruption.TestAttemptId == candidate.TestAttempt.Id
                                         && interruption.StatusId == TestAttemptInterruptionStatusIds.Open),
                candidate.TestSession.Status!.BackendName))
            .ToListAsync(ct);

        return Result.Ok(new PaginatedResult<TestSlotCandidateListItemDto>(
            items, totalCount, request.PageNumber, request.PageSize));
    }
}
