using System.Text.Json;
using Application.Operation.Features.Employee.Interview.Committee.Commands;
using Application.Operation.Features.Employee.Interview.Committee.Services;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Interview;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.Interview.Committee.Handlers.Commands;

public sealed class UpdateCommitteeCommandHandler(
    IUnitOfWork unitOfWork, UserManager<User> userManager, CommitteeMemberEligibilityService eligibility)
    : IRequestHandler<UpdateCommitteeCommand, IResult<Unit>>
{
    private sealed record CommitteeCoreSnapshot(string NameAr, string? NameEn, string? ScopeDescription, string? Notes);

    public async Task<IResult<Unit>> Handle(UpdateCommitteeCommand request, CancellationToken cancellationToken)
    {
        var committee = await unitOfWork.GetEntityRepository<InterviewCommittee>().DbSet
            .Include(c => c.Members).ThenInclude(m => m.EvaluationAxes)
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
        if (committee is null)
            return Result.Fail<Unit>(new Error(ErrorsCodes.InterviewCommitteeNotFound));

        var oldSnapshot = new CommitteeCoreSnapshot(committee.NameAr, committee.NameEn, committee.ScopeDescription, committee.Notes);
        var beforeActiveUserIds = committee.Members.Where(m => m.IsActive).Select(m => m.MemberUserId).ToHashSet();

        var updateResult = committee.Update(request.NameAr, request.NameEn, request.ScopeDescription, request.Notes);
        if (updateResult.IsFailed)
            return Result.Fail<Unit>(updateResult.Errors);

        var memberUserIds = request.Members.Select(m => m.MemberUserId).ToList();
        if (memberUserIds.Count > 0)
        {
            var existingUserCount = await userManager.Users
                .CountAsync(u => memberUserIds.Contains(u.Id), cancellationToken);
            if (existingUserCount != memberUserIds.Distinct().Count())
                return Result.Fail<Unit>(new Error(ErrorsCodes.UserNotFound));

            if (!await eligibility.AreAllEligibleAsync(memberUserIds, cancellationToken))
                return Result.Fail<Unit>(new Error(ErrorsCodes.InterviewCommitteeMemberNotEligible));
        }

        var allAxisIds = request.Members.SelectMany(m => m.EvaluationAxisIds).Distinct().ToList();
        if (allAxisIds.Count > 0)
        {
            // Assignable axes come from whichever version of this template is currently Approved -
            // that's the rubric actually in effect.
            var approvedVersion = await unitOfWork.GetEntityRepository<InterviewTemplateVersion>().DbSet
                .FirstOrDefaultAsync(
                    v => v.InterviewTemplateId == committee.InterviewTemplateId && v.Status == TemplateVersionStatus.Approved,
                    cancellationToken);
            if (approvedVersion is null)
                return Result.Fail<Unit>(new Error(ErrorsCodes.InterviewTemplateHasNoApprovedVersion));

            var validAxisCount = await unitOfWork.GetEntityRepository<InterviewTemplateEvaluationAxis>().DbSet
                .CountAsync(a => allAxisIds.Contains(a.Id) && a.InterviewTemplateVersionId == approvedVersion.Id, cancellationToken);
            if (validAxisCount != allAxisIds.Count)
                return Result.Fail<Unit>(new Error(ErrorsCodes.InterviewTemplateVersionAxisNotFound));
        }

        var memberInputs = request.Members
            .Select(m => new CommitteeMemberInput(
                m.MemberUserId,
                m.Role,
                m.ParticipatesInEvaluation,
                m.CanViewCandidates,
                m.CanAddNotes,
                m.CanSubmitEvaluation,
                m.CanViewOtherEvaluations,
                m.CanViewCommitteeSummary,
                m.EvaluationAxisIds))
            .ToList();

        var setMembersResult = committee.SetMembers(memberInputs);
        if (setMembersResult.IsFailed)
            return Result.Fail<Unit>(setMembersResult.Errors);

        var auditRepo = unitOfWork.GetEntityRepository<InterviewAuditLog>();

        var newSnapshot = new CommitteeCoreSnapshot(committee.NameAr, committee.NameEn, committee.ScopeDescription, committee.Notes);
        await auditRepo.AddAsync(new InterviewAuditLog
        {
            EntityType = nameof(InterviewCommittee),
            EntityId = committee.Id,
            Action = InterviewCommitteeAuditActions.Updated,
            OldValues = JsonSerializer.Serialize(oldSnapshot),
            NewValues = JsonSerializer.Serialize(newSnapshot)
        }, cancellationToken);

        // AddMember/Remove already raised the CommitteeMemberAdded/RemovedEvent domain events for
        // future notifications - this is a separate, human-readable audit trail of the same diff.
        var afterActiveUserIds = committee.Members.Where(m => m.IsActive).Select(m => m.MemberUserId).ToHashSet();
        var addedUserIds = afterActiveUserIds.Except(beforeActiveUserIds).ToHashSet();
        var removedUserIds = beforeActiveUserIds.Except(afterActiveUserIds).ToHashSet();

        foreach (var member in committee.Members.Where(m => addedUserIds.Contains(m.MemberUserId)))
        {
            await auditRepo.AddAsync(new InterviewAuditLog
            {
                EntityType = nameof(InterviewCommitteeMember),
                EntityId = member.Id,
                Action = InterviewCommitteeAuditActions.MemberAdded,
                NewValues = JsonSerializer.Serialize(new { member.MemberUserId, member.Role })
            }, cancellationToken);
        }

        foreach (var member in committee.Members.Where(m => removedUserIds.Contains(m.MemberUserId)))
        {
            await auditRepo.AddAsync(new InterviewAuditLog
            {
                EntityType = nameof(InterviewCommitteeMember),
                EntityId = member.Id,
                Action = InterviewCommitteeAuditActions.MemberRemoved,
                OldValues = JsonSerializer.Serialize(new { member.MemberUserId, member.Role }),
                Reason = member.RemovalReason
            }, cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(Unit.Value);
    }
}
