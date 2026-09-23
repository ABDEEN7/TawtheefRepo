using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Interview;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.Interview.Evaluation.Services;

// Resource-owner resolution for the Member Evaluation feature. InterviewEvaluation.{View,Manage}
// is seeded only onto HrManager/EmployeeSuperAdmin, same as every other Interview permission pair -
// an admin who wants a plain committee member (Employee/DepartmentManager) to submit their own
// evaluations grants it via a custom role created on the Roles admin page instead of it being baked
// onto a system role (system roles - IsSystemRole=true - can't be edited there at all). Whoever ends
// up holding the permission, the attribute only opens the endpoint category; this resolver is what
// actually restricts a caller to their own assigned appointment. Mirrors the existing
// EmployeeJobAccessContextProvider/EmployeeJobAccessScope idiom used for Job self-service access.
public sealed class EvaluationAccessResolver(
    IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IHttpContextAccessor httpContextAccessor)
{
    public Guid? GetCurrentUserId() => Guid.TryParse(currentUserService.UserId, out var id) ? id : null;

    // requireCanSubmit=true (the default) is used everywhere the caller is about to view/edit their
    // own scoring form - CanSubmitEvaluation is the flag this repo already uses to gate whether a
    // committee member may score at all.
    public async Task<Result<InterviewCommitteeMember>> GetAssignedMemberAsync(
        Guid interviewCommitteeId, CancellationToken cancellationToken, bool requireCanSubmit = true)
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId is null)
            return Forbidden();

        var member = await unitOfWork.GetEntityRepository<InterviewCommitteeMember>().DbSet
            .Include(m => m.EvaluationAxes)
            .FirstOrDefaultAsync(m => m.InterviewCommitteeId == interviewCommitteeId
                && m.MemberUserId == currentUserId.Value && m.IsActive, cancellationToken);

        if (member is null || (requireCanSubmit && !member.CanSubmitEvaluation))
            return Forbidden();

        return Result.Ok(member);

        static Result<InterviewCommitteeMember> Forbidden() =>
            Result.Fail<InterviewCommitteeMember>(new Error(ErrorsCodes.InterviewMemberEvaluationNotAssignedToAppointment)
                .WithMetadata("StatusCode", StatusCodes.Status403Forbidden));
    }

    // Chair/HR-summary bypass: HrManager/EmployeeSuperAdmin can view any committee's summary
    // regardless of their own membership row, same shape as ClaimsPrincipalExtensions.HasFullJobAccess.
    public bool HasSummaryRoleBypass()
    {
        var user = httpContextAccessor.HttpContext?.User;
        return user is not null
            && (user.IsInRole(nameof(SystemRoleIds.HrManager)) || user.IsInRole(nameof(SystemRoleIds.EmployeeSuperAdmin)));
    }

    // Attendance registration / starting the interview are chair-administrative actions, distinct
    // from GetAssignedMemberAsync's "own scoring form" ownership rule - a Chair may not even have
    // CanSubmitEvaluation. Reuses the same HR/SuperAdmin bypass as the committee-summary rule.
    public async Task<Result> EnsureChairOrBypassAsync(Guid interviewCommitteeId, CancellationToken cancellationToken)
    {
        if (HasSummaryRoleBypass())
            return Result.Ok();

        var currentUserId = GetCurrentUserId();
        if (currentUserId is null)
            return Forbidden();

        var isChair = await unitOfWork.GetEntityRepository<InterviewCommitteeMember>().DbSet
            .AnyAsync(m => m.InterviewCommitteeId == interviewCommitteeId
                && m.MemberUserId == currentUserId.Value && m.IsActive && m.Role == CommitteeRole.Chair, cancellationToken);

        return isChair ? Result.Ok() : Forbidden();

        static Result Forbidden() =>
            Result.Fail(new Error(ErrorsCodes.InterviewAppointmentAttendanceForbidden)
                .WithMetadata("StatusCode", StatusCodes.Status403Forbidden));
    }
}
