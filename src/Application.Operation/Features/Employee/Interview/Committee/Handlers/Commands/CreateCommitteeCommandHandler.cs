using MediatR;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Interview;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;
using Application.Operation.Features.Employee.Interview.Committee.Commands;

namespace Application.Operation.Features.Employee.Interview.Committee.Handlers.Commands;

public sealed class CreateCommitteeCommandHandler(IUnitOfWork unitOfWork, UserManager<User> userManager)
    : IRequestHandler<CreateCommitteeCommand, IResult<Guid>>
{
    public async Task<IResult<Guid>> Handle(CreateCommitteeCommand request, CancellationToken cancellationToken)
    {
        var job = await unitOfWork.GetEntityRepository<Job>().DbSet
            .FirstOrDefaultAsync(j => j.Id == request.JobId, cancellationToken);
        if (job is null)
            return Result.Fail<Guid>(new Error(ErrorsCodes.JobNotFound));

        if (job.JobStatusId != JobStatusIds.Published)
            return Result.Fail<Guid>(new Error(ErrorsCodes.InterviewCommitteeJobNotPublished));

        var templateExists = await unitOfWork.GetEntityRepository<InterviewTemplate>().DbSet
            .AnyAsync(t => t.Id == request.InterviewTemplateId, cancellationToken);
        if (!templateExists)
            return Result.Fail<Guid>(new Error(ErrorsCodes.InterviewTemplateNotFound));

        var committeeRepo = unitOfWork.GetEntityRepository<InterviewCommittee>();

        var jobAlreadyHasActiveCommittee = await committeeRepo.DbSet
            .AnyAsync(c => c.JobId == request.JobId && c.IsActive, cancellationToken);
        if (jobAlreadyHasActiveCommittee)
            return Result.Fail<Guid>(new Error(ErrorsCodes.InterviewCommitteeAlreadyExistsForJob));

        var memberUserIds = request.Members.Select(m => m.MemberUserId).ToList();
        if (memberUserIds.Count > 0)
        {
            var existingUserCount = await userManager.Users
                .CountAsync(u => memberUserIds.Contains(u.Id), cancellationToken);
            if (existingUserCount != memberUserIds.Distinct().Count())
                return Result.Fail<Guid>(new Error(ErrorsCodes.UserNotFound));
        }

        var allAxisIds = request.Members.SelectMany(m => m.EvaluationAxisIds).Distinct().ToList();
        if (allAxisIds.Count > 0)
        {
            // Assignable axes come from whichever version of this template is currently Approved -
            // that's the rubric actually in effect.
            var approvedVersion = await unitOfWork.GetEntityRepository<InterviewTemplateVersion>().DbSet
                .FirstOrDefaultAsync(
                    v => v.InterviewTemplateId == request.InterviewTemplateId && v.Status == TemplateVersionStatus.Approved,
                    cancellationToken);
            if (approvedVersion is null)
                return Result.Fail<Guid>(new Error(ErrorsCodes.InterviewTemplateHasNoApprovedVersion));

            var validAxisCount = await unitOfWork.GetEntityRepository<InterviewTemplateEvaluationAxis>().DbSet
                .CountAsync(a => allAxisIds.Contains(a.Id) && a.InterviewTemplateVersionId == approvedVersion.Id, cancellationToken);
            if (validAxisCount != allAxisIds.Count)
                return Result.Fail<Guid>(new Error(ErrorsCodes.InterviewTemplateVersionAxisNotFound));
        }

        var committee = InterviewCommittee.Create(
            request.JobId,
            request.InterviewTemplateId,
            request.NameAr,
            request.NameEn,
            request.ScopeDescription,
            request.Notes);

        await committeeRepo.AddAsync(committee);

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
            return Result.Fail<Guid>(setMembersResult.Errors);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(committee.Id);
    }
}
