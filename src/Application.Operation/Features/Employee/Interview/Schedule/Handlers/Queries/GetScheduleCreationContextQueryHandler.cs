using Application.Operation.Features.Employee.Interview.Schedule.DTOs;
using Application.Operation.Features.Employee.Interview.Schedule.Queries;
using Application.Operation.Features.Employee.Interview.Schedule.Services;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Interview;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.Interview.Schedule.Handlers.Queries;

public sealed class GetScheduleCreationContextQueryHandler(IUnitOfWork unitOfWork, UserManager<User> userManager)
    : IRequestHandler<GetScheduleCreationContextQuery, IResult<ScheduleCreationContextDto>>
{
    public async Task<IResult<ScheduleCreationContextDto>> Handle(GetScheduleCreationContextQuery request, CancellationToken cancellationToken)
    {
        var job = await unitOfWork.GetEntityRepository<Job>().DbSet
            .AsNoTracking()
            .Include(j => j.JobTitle)
            .Include(j => j.Major)
            .Include(j => j.Department)
            .FirstOrDefaultAsync(j => j.Id == request.JobId, cancellationToken);
        if (job is null)
            return Result.Fail<ScheduleCreationContextDto>(new Error(ErrorsCodes.JobNotFound));

        var committeeResult = await ScheduleEligibilityResolver.GetApprovedCommitteeAsync(unitOfWork, request.JobId, cancellationToken);
        if (committeeResult.IsFailed)
            return Result.Fail<ScheduleCreationContextDto>(committeeResult.Errors);
        var committee = committeeResult.Value;

        var template = await unitOfWork.GetEntityRepository<InterviewTemplate>().DbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == committee.InterviewTemplateId, cancellationToken);
        if (template is null)
            return Result.Fail<ScheduleCreationContextDto>(new Error(ErrorsCodes.InterviewTemplateNotFound));

        var hasApprovedVersion = await unitOfWork.GetEntityRepository<InterviewTemplateVersion>().DbSet
            .AnyAsync(v => v.InterviewTemplateId == committee.InterviewTemplateId && v.Status == TemplateVersionStatus.Approved, cancellationToken);

        var memberRows = await unitOfWork.GetEntityRepository<InterviewCommitteeMember>().DbSet
            .AsNoTracking()
            .Where(m => m.InterviewCommitteeId == committee.Id && m.IsActive)
            .Select(m => new { m.MemberUserId, m.Role })
            .ToListAsync(cancellationToken);

        // Two round trips, matched in memory - User is IdentityUser-based, not an EventEntity, so
        // it can't be queried via the generic repository / joined server-side against it.
        var memberUserIds = memberRows.Select(m => m.MemberUserId).ToList();
        var users = await userManager.Users.Where(u => memberUserIds.Contains(u.Id)).ToListAsync(cancellationToken);

        var members = memberRows
            .Select(m =>
            {
                var user = users.FirstOrDefault(u => u.Id == m.MemberUserId);
                return new ScheduleCommitteeMemberDto(m.MemberUserId, user?.FullNameAr ?? string.Empty, user?.FullNameEn, m.Role);
            })
            .ToList();

        var eligibleCandidates = await ScheduleEligibilityResolver.GetEligibleCandidatesAsync(
            unitOfWork, request.JobId, cancellationToken, excludeScheduleId: request.ExcludeScheduleId);

        var dto = new ScheduleCreationContextDto(
            job.Id,
            job.JobTitle!.JobNameAr,
            job.JobTitle.JobNameEn,
            job.Major?.NameAr,
            job.Major?.NameEn,
            job.Department?.NameAr,
            job.Department?.NameEn,
            committee.InterviewTemplateId,
            template.TitleAr,
            template.TitleEn,
            hasApprovedVersion,
            committee.Id,
            committee.NameAr,
            committee.NameEn,
            committee.Status,
            members,
            eligibleCandidates.Count,
            eligibleCandidates.Count(c => c.GenderId == GenderIds.Male),
            eligibleCandidates.Count(c => c.GenderId == GenderIds.Female));

        return Result.Ok(dto);
    }
}
