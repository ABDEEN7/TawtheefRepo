using Application.Operation.Features.Employee.Common.Access;
using Application.Operation.Features.Employee.JobManagement.JobInvitationSummary.DTOs;
using Application.Operation.Features.Employee.JobManagement.JobInvitationSummary.Queries;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Common.Models;
using Tawtheef.Application.Common.Models.Filters;
using Tawtheef.Application.Extensions;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment;

using Application.Operation.Features.Employee.JobManagement.JobInvitationSummary.Contracts;

namespace Application.Operation.Features.Employee.JobManagement.JobInvitationSummary.Services;

internal sealed class JobInvitationSummaryQueryBuilder(
    IUnitOfWork unitOfWork,
    ILocalizationService localizationService,
    EmployeeJobAccessContextProvider accessContextProvider)
{
    public IQueryable<JobInvitationSummaryDto> Build(IJobInvitationSummaryFilter filter)
    {
        var language = localizationService.GetCurrentLanguage();
        var hasValidYear = YearRange.TryCreate(filter.Year, out var yearRange);

        return unitOfWork.GetEntityRepository<Job>().DbSet
            .AsNoTracking()
            .ApplyJobAccessScope(accessContextProvider.GetAccess())
            .WhereIf(hasValidYear, job =>
                job.CreatedDate >= yearRange.FromUtc && job.CreatedDate < yearRange.ToExclusiveUtc)
            .WhereIf(filter.JobCategoryId is not null,
                job => job.JobCategoryId == filter.JobCategoryId)
            .WhereIf(filter.DepartmentId is not null,
                job => job.DepartmentId == filter.DepartmentId)
            .WhereIf(filter.JobStatusId is not null,
                job => job.JobStatusId == filter.JobStatusId)
            .WhereIf(!string.IsNullOrWhiteSpace(filter.Search), job =>
                EF.Functions.Like(job.JobTitle!.JobNameEn, $"%{filter.Search}%") ||
                EF.Functions.Like(job.JobTitle!.JobNameAr, $"%{filter.Search}%"))
            .Select(job => new JobInvitationSummaryDto
            {
                JobId = job.Id,
                JobStatus = new DropdownOptions
                {
                    Id = job.JobStatus!.Id,
                    Name = language == "en" ? job.JobStatus.NameEn : job.JobStatus.NameAr,
                    BackendName = job.JobStatus.BackendName
                },
                InvitationCount = job.Invitations.Count,
                ApplicantsCount = job.Invitations.Count(invitation =>
                    invitation.InvitationStatusId == InvitationStatusIds.ExamEligible),
                RefusedCount = job.Invitations.Count(invitation =>
                    invitation.InvitationStatusId == InvitationStatusIds.Rejected),
                NotSeenCount = job.Invitations.Count(invitation =>
                    invitation.InvitationStatusId == InvitationStatusIds.NewInvitation),
                ReadCount = job.Invitations.Count(invitation =>
                    invitation.InvitationStatusId == InvitationStatusIds.Read ||
                    invitation.InvitationStatusId == InvitationStatusIds.PendingAttachmentApproval ||
                    invitation.InvitationStatusId == InvitationStatusIds.ReturnedAttachment ||
                    invitation.InvitationStatusId == InvitationStatusIds.ExamEligible ||
                    invitation.InvitationStatusId == InvitationStatusIds.Rejected),
                ExpiredCount = job.Invitations.Count(invitation =>
                    invitation.InvitationStatusId == InvitationStatusIds.Closed),
                CancelledCount = job.Invitations.Count(invitation =>
                    invitation.InvitationStatusId == InvitationStatusIds.Cancelled),
                PendingAttachmentApprovalCount = job.Invitations.Count(invitation =>
                    invitation.InvitationStatusId == InvitationStatusIds.PendingAttachmentApproval),
                ReturnedAttachmentCount = job.Invitations.Count(invitation =>
                    invitation.InvitationStatusId == InvitationStatusIds.ReturnedAttachment),
                CreateDate = job.PublishAt ?? job.UpdatedDate ?? job.CreatedDate,
                JobName = language == "en" ? job.JobTitle!.JobNameEn : job.JobTitle!.JobNameAr,
                DepartmentName = language == "en" ? job.Department!.NameEn : job.Department!.NameAr,
                JobCategory = language == "en" ? job.JobCategory!.NameEn : job.JobCategory!.NameAr,
                LastBatchNumber = job.Invitations
                    .OrderByDescending(invitation => invitation.CreatedDate)
                    .Select(invitation => (Guid?)invitation.BatchNumber)
                    .FirstOrDefault(),
                PreviousBatchInvitations = job.Invitations
                    .OrderByDescending(invitation => invitation.CreatedDate)
                    .Select(invitation => invitation.BatchNumber)
                    .Take(1)
                    .Select(lastBatch => job.Invitations.Count(invitation =>
                        invitation.BatchNumber != lastBatch))
                    .FirstOrDefault()
            });
    }

    public IQueryable<JobInvitationSummaryDto> ApplySorting(
        IQueryable<JobInvitationSummaryDto> query,
        string? sortBy,
        string? sortDirection)
    {
        var descending = string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase);
        return sortBy?.ToLowerInvariant() switch
        {
            "jobname" => descending ? query.OrderByDescending(row => row.JobName) : query.OrderBy(row => row.JobName),
            "jobstatus.name" => descending
                ? query.OrderByDescending(row => row.JobStatus.Name)
                : query.OrderBy(row => row.JobStatus.Name),
            "invitationcount" => descending
                ? query.OrderByDescending(row => row.InvitationCount)
                : query.OrderBy(row => row.InvitationCount),
            "applicantscount" => descending
                ? query.OrderByDescending(row => row.ApplicantsCount)
                : query.OrderBy(row => row.ApplicantsCount),
            _ => query.OrderByDescending(row => row.CreateDate)
        };
    }
}
