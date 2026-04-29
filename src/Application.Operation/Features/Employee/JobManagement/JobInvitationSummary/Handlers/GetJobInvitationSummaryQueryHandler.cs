using Application.Operation.Features.Employee.JobManagement.JobInvitationSummary.DTOs;
using Application.Operation.Features.Employee.JobManagement.JobInvitationSummary.Queries;
using MediatR;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Common.Models;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Extensions;
using Tawtheef.Domain.Entities.Lookups;

namespace Application.Operation.Features.Employee.JobManagement.JobInvitationSummary.Handlers;

public sealed class GetJobInvitationSummaryQueryHandler(IUnitOfWork unitOfWork, ILocalizationService localizationService)
    : IRequestHandler<GetJobInvitationSummaryQuery, IResult<PaginatedResult<JobInvitationSummaryDto>>>
{
    public async Task<IResult<PaginatedResult<JobInvitationSummaryDto>>> Handle(GetJobInvitationSummaryQuery query,
        CancellationToken cancellationToken)
    {
        var language = localizationService.GetCurrentLanguage();

        var queryable = unitOfWork
            .GetEntityRepository<Tawtheef.Domain.Entities.Recruitment.Job>()
            .DbSet
            .AsNoTracking()
            .WhereIf(query.JobCategoryId is not null, i => i.JobCategoryId == query.JobCategoryId)
            .WhereIf(query.DepartmentId is not null, i => i.DepartmentId == query.DepartmentId)
            .WhereIf(query.JobStatusId is not null, i => i.JobStatusId == query.JobStatusId)
            .Select(job => new JobInvitationSummaryDto {
                JobId = job.Id,
                JobStatus = new DropdownOptions
                {
                    Id = job.JobStatus!.Id,
                    Name = language == "en"
                        ? job.JobStatus.NameEn
                        : job.JobStatus.NameAr,
                    BackendName = job.JobStatus.BackendName
                },
                InvitationCount = job.Invitations.Count,
                ApplicantsCount = job.Invitations
                    .Count(i => i.InvitationStatusId == InvitationStatusIds.ExamEligible),
                RefusedCount = job.Invitations
                    .Count(i => i.InvitationStatusId == InvitationStatusIds.Rejected),
                NotSeenCount = job.Invitations
                    .Count(i => i.InvitationStatusId == InvitationStatusIds.NewInvitation),
                ReadCount = job.Invitations.Count(i => 
                    i.InvitationStatusId == InvitationStatusIds.Read ||
                    i.InvitationStatusId == InvitationStatusIds.PendingAttachmentApproval ||
                    i.InvitationStatusId == InvitationStatusIds.ReturnedAttachment ||
                    i.InvitationStatusId == InvitationStatusIds.ExamEligible ||
                    i.InvitationStatusId == InvitationStatusIds.Rejected),
                ExpiredCount = job.Invitations
                    .Count(i => i.InvitationStatusId == InvitationStatusIds.Closed),
                CancelledCount = job.Invitations
                    .Count(i => i.InvitationStatusId == InvitationStatusIds.Cancelled),
                PendingAttachmentApprovalCount = job.Invitations
                    .Count(i => i.InvitationStatusId == InvitationStatusIds.PendingAttachmentApproval),
                ReturnedAttachmentCount = job.Invitations
                    .Count(i => i.InvitationStatusId == InvitationStatusIds.ReturnedAttachment),
                CreateDate = job.PublishAt ?? job.UpdatedDate ?? job.CreatedDate,
                JobName = language == "en" ? job.JobTitle!.JobNameEn : job.JobTitle!.JobNameAr,
                DepartmentName = language == "en" ? job.Department!.NameEn : job.Department!.NameAr,
                JobCategory = language == "en" ? job.JobCategory!.NameEn : job.JobCategory!.NameAr,
                LastBatchNumber = job.Invitations
                    .OrderByDescending(i => i.CreatedDate)
                    .Select(i => (Guid?)i.BatchNumber)
                    .FirstOrDefault(),
                PreviousBatchInvitations =
                    job.Invitations
                        .OrderByDescending(i => i.CreatedDate)
                        .Select(i => i.BatchNumber)
                        .Take(1)
                        .Select(lastBatch =>
                            job.Invitations.Count(i => i.BatchNumber != lastBatch))
                        .FirstOrDefault()
            })
            .OrderByDescending(i=> i.CreateDate);

        var result = await queryable.ToPaginatedListAsync(query, cancellationToken);

        return Result.Ok(result);
    }
}

