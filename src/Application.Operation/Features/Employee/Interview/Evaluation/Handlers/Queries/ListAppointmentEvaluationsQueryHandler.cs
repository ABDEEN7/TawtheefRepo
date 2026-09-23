using Application.Operation.Features.Employee.Interview.Evaluation.DTOs;
using Application.Operation.Features.Employee.Interview.Evaluation.Queries;
using Application.Operation.Features.Employee.Interview.Evaluation.Services;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Extensions;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Interview;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.Interview.Evaluation.Handlers.Queries;

public sealed class ListAppointmentEvaluationsQueryHandler(
    IUnitOfWork unitOfWork, EvaluationAccessResolver accessResolver, UserManager<User> userManager)
    : IRequestHandler<ListAppointmentEvaluationsQuery, IResult<AppointmentEvaluationSummaryDto>>
{
    public async Task<IResult<AppointmentEvaluationSummaryDto>> Handle(ListAppointmentEvaluationsQuery request, CancellationToken cancellationToken)
    {
        var appointment = await unitOfWork.GetEntityRepository<InterviewAppointment>().DbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == request.AppointmentId, cancellationToken);
        if (appointment is null)
            return Result.Fail<AppointmentEvaluationSummaryDto>(new Error(ErrorsCodes.InterviewAppointmentNotFound));

        // Distinct from the "my own form" ownership rule: Chair, CanViewCommitteeSummary members, or
        // an HrManager/EmployeeSuperAdmin role bypass may view the whole committee's progress.
        if (!accessResolver.HasSummaryRoleBypass())
        {
            var currentUserId = accessResolver.GetCurrentUserId();
            var viewerMember = currentUserId is null
                ? null
                : await unitOfWork.GetEntityRepository<InterviewCommitteeMember>().DbSet
                    .AsNoTracking()
                    .FirstOrDefaultAsync(m => m.InterviewCommitteeId == appointment.InterviewCommitteeId
                        && m.MemberUserId == currentUserId.Value && m.IsActive, cancellationToken);

            var canView = viewerMember is not null && (viewerMember.Role == CommitteeRole.Chair || viewerMember.CanViewCommitteeSummary);
            if (!canView)
                return Result.Fail<AppointmentEvaluationSummaryDto>(new Error(ErrorsCodes.InterviewMemberEvaluationSummaryAccessDenied)
                    .WithMetadata("StatusCode", StatusCodes.Status403Forbidden));
        }

        var members = await unitOfWork.GetEntityRepository<InterviewCommitteeMember>().DbSet
            .AsNoTracking()
            .Where(m => m.InterviewCommitteeId == appointment.InterviewCommitteeId && m.IsActive)
            .Select(m => new { m.Id, m.MemberUserId, m.Role })
            .ToListAsync(cancellationToken);

        // Two round trips, matched in memory - User is IdentityUser-based, not an EventEntity, so it
        // can't be queried via the generic repository / joined server-side against it.
        var memberUserIds = members.Select(m => m.MemberUserId).ToList();
        var users = await userManager.Users.Where(u => memberUserIds.Contains(u.Id)).ToListAsync(cancellationToken);

        var evaluations = await unitOfWork.GetEntityRepository<InterviewMemberEvaluation>().DbSet
            .AsNoTracking()
            .Where(e => e.InterviewAppointmentId == appointment.Id)
            .ToListAsync(cancellationToken);

        var memberDtos = members
            .Select(m =>
            {
                var user = users.FirstOrDefault(u => u.Id == m.MemberUserId);
                var evaluation = evaluations.FirstOrDefault(e => e.InterviewCommitteeMemberId == m.Id);
                return new MemberEvaluationSummaryDto(
                    m.Id,
                    m.MemberUserId,
                    user?.FullNameAr ?? string.Empty,
                    user?.FullNameEn,
                    m.Role,
                    evaluation?.Status,
                    evaluation?.TotalScore,
                    evaluation?.SubmittedAt.AsUtcOffset());
            })
            .ToList();

        return Result.Ok(new AppointmentEvaluationSummaryDto(appointment.Id, appointment.Status, memberDtos));
    }
}
