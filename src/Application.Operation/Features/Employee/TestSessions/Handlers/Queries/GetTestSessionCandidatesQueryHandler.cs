using Application.Operation.Features.Employee.JobManagement.JobCandidates.Services;
using Application.Operation.Features.Employee.TestSessions.DTOs;
using Application.Operation.Features.Employee.TestSessions.Queries;
using Application.Operation.Features.Employee.TestSessions.Services;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Extensions;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Applicant;
using Tawtheef.Domain.Entities.Exams;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.TestSessions.Handlers.Queries;

public sealed class GetTestSessionCandidatesQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetTestSessionCandidatesQuery, IResult<TestSessionCandidatesDto>>
{
    public async Task<IResult<TestSessionCandidatesDto>> Handle(
        GetTestSessionCandidatesQuery request,
        CancellationToken ct)
    {
        var isArabic = string.Equals(request.Language, "ar", StringComparison.OrdinalIgnoreCase);
        var jobId = await unitOfWork.Context.Set<Exam>().AsNoTracking()
            .Where(exam => exam.Id == request.ExamId && exam.StatusId == ExamStatusIds.Approved)
            .Select(exam => (Guid?)exam.JobId)
            .FirstOrDefaultAsync(ct);

        if (jobId is null)
            return Result.Fail<TestSessionCandidatesDto>(ErrorsCodes.InvalidRequest);

        var currentTestSessionId = request.TestSessionId ?? Guid.Empty;
        var unavailableInvitationIds = TestSessionCandidateConflictService
            .BlockingAssignments(unitOfWork.Context, jobId.Value, currentTestSessionId)
            .Select(candidate => candidate.InvitationId);

        var invitations = unitOfWork.Context.Set<Invitation>().AsNoTracking()
            .Where(invitation => invitation.JobId == jobId.Value)
            .Where(invitation => !unavailableInvitationIds.Contains(invitation.Id) ||
                                 request.TestSessionId.HasValue &&
                                 unitOfWork.Context.Set<TestSessionCandidate>().Any(candidate =>
                                     candidate.TestSessionId == request.TestSessionId.Value &&
                                     candidate.InvitationId == invitation.Id))
            .WhereIf(request.GenderFilter.HasValue, invitation =>
                invitation.Applicant!.Profile!.GenderId ==
                (request.GenderFilter == TestSessionGenderFilter.Male ? GenderIds.Male : GenderIds.Female))
            .WhereIf(request.NationalityFilter.HasValue, invitation =>
                request.NationalityFilter == TestSessionNationalityFilter.Qatari
                    ? invitation.Applicant!.Profile!.NationalityId == CountryIds.Qatar
                    : invitation.Applicant!.Profile!.NationalityId.HasValue &&
                      invitation.Applicant.Profile.NationalityId != CountryIds.Qatar);

        var candidates = invitations.Where(invitation =>
            invitation.Source == InvitationSource.Exceptional ||
            (invitation.Source != InvitationSource.Exceptional &&
             (invitation.InvitationStatusId == InvitationStatusIds.NewInvitation ||
              invitation.InvitationStatusId == InvitationStatusIds.Read ||
              (invitation.InvitationStatusId == InvitationStatusIds.ExamEligible &&
               invitation.Applicant!.Profile != null &&
               invitation.Applicant.Profile.Status == UserProfileStatus.Approved &&
               invitation.Applicant.Profile.AvailableForRecruitment))));

        var total = await candidates.CountAsync(ct);
        var eligible = await candidates.CountAsync(invitation =>
            invitation.Source != InvitationSource.Exceptional &&
            invitation.InvitationStatusId == InvitationStatusIds.ExamEligible &&
            invitation.Applicant!.Profile != null &&
            invitation.Applicant.Profile.Status == UserProfileStatus.Approved &&
            invitation.Applicant.Profile.AvailableForRecruitment, ct);
        var notReady = await candidates.CountAsync(invitation =>
            invitation.Source != InvitationSource.Exceptional &&
            (invitation.InvitationStatusId == InvitationStatusIds.NewInvitation ||
             invitation.InvitationStatusId == InvitationStatusIds.Read), ct);
        var excluded = await candidates.CountAsync(
            invitation => invitation.Source == InvitationSource.Exceptional,
            ct);

        var items = await candidates
            .OrderBy(invitation => invitation.Applicant!.FullNameAr)
            .ThenBy(invitation => invitation.Id)
            .Select(TestSessionCandidateProjection.ForLanguage(isArabic))
            .ToListAsync(ct);

        return Result.Ok(new TestSessionCandidatesDto(
            items,
            new TestSessionCandidateSummaryDto(total, eligible, notReady, excluded)));
    }
}
