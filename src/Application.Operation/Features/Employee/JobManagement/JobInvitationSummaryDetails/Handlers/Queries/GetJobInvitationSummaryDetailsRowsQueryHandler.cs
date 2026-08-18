using Application.Operation.Features.Employee.Common.Access;
using Application.Operation.Features.Employee.JobManagement.JobInvitationSummaryDetails.DTOs;
using Application.Operation.Features.Employee.JobManagement.JobInvitationSummaryDetails.Queries;
using MediatR;
using FluentResults;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Common.Models;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Extensions;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment;

namespace Application.Operation.Features.Employee.JobManagement.JobInvitationSummaryDetails.Handlers.Queries;

public sealed class GetJobInvitationSummaryDetailsRowsQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ILocalizationService localizationService,
    EmployeeJobAccessContextProvider accessContextProvider)
    : IRequestHandler<GetJobInvitationSummaryDetailsRowsQuery, IResult<PaginatedResult<JobInvitationSummaryDetailsRowDto>>>
{
    public async Task<IResult<PaginatedResult<JobInvitationSummaryDetailsRowDto>>> Handle(
        GetJobInvitationSummaryDetailsRowsQuery query,
        CancellationToken cancellationToken)
    {
        var canAccessJob = await unitOfWork.GetEntityRepository<Job>().DbSet
            .AsNoTracking()
            .ApplyJobAccessScope(accessContextProvider.GetAccess())
            .AnyAsync(job => job.Id == query.JobId, cancellationToken);
        if (!canAccessJob)
            return Result.Fail<PaginatedResult<JobInvitationSummaryDetailsRowDto>>(JobMessages.JobNotFound);

        var searchTerm = query.Search?.Trim();

        var invitations = unitOfWork.GetEntityRepository<Invitation>().DbSet
            .AsNoTracking()
            .Include(invitation => invitation.InvitationStatus)
            .Include(invitation => invitation.Attachments)
            .Include(invitation => invitation.Applicant).ThenInclude(applicant => applicant!.Profile)
            .ThenInclude(profile => profile!.Nationality)
            .Where(invitation => invitation.JobId == query.JobId)
            .WhereIf(query.StatusId.HasValue, invitation => invitation.InvitationStatusId == query.StatusId!.Value)
            .WhereIf(query.BatchNumber.HasValue, invitation => invitation.BatchNumber == query.BatchNumber!.Value)
            .WhereIf(!string.IsNullOrWhiteSpace(searchTerm), invitation =>
                (invitation.Applicant != null &&
                 (invitation.Applicant.FullNameAr.Contains(searchTerm!) ||
                  invitation.Applicant.FullNameEn.Contains(searchTerm!) ||
                  (invitation.Applicant.PhoneNumber != null &&
                   invitation.Applicant.PhoneNumber.Contains(searchTerm!))
                  )) ||
                (invitation.Applicant != null &&
                 invitation.Applicant.Profile != null &&
                 invitation.Applicant.Profile.NationalNumber != null &&
                 invitation.Applicant.Profile.NationalNumber.Contains(searchTerm!)));

        var pagedInvitations = await invitations.ToPaginatedListAsync(query, cancellationToken);
        var items = mapper.Map<List<JobInvitationSummaryDetailsRowDto>>(pagedInvitations.Items);

        for (var index = 0; index < items.Count; index++)
        {
            var invitation = pagedInvitations.Items[index];
            var item = items[index];
            item.FullName = localizationService.GetLocalizedFullName(invitation.Applicant);
            item.Nationality = localizationService.GetLocalizedName(invitation.Applicant?.Profile?.Nationality);
            item.PersonalNumber = invitation.Applicant?.Profile?.NationalNumber ?? string.Empty;
            item.Status = invitation.InvitationStatus == null
                ? new DropdownOptions()
                : new DropdownOptions
                {
                    Id = invitation.InvitationStatus.Id,
                    BackendName = invitation.InvitationStatus.BackendName,
                    Name = localizationService.GetLocalizedName(invitation.InvitationStatus),
                    Description = localizationService.GetLocalizedDescription(invitation.InvitationStatus),
                    AdditionalData = new
                    {
                        invitation.InvitationStatus.NameAr,
                        invitation.InvitationStatus.NameEn,
                        invitation.InvitationStatus.DisplayOrder
                    }
                };
            item.ReadDate = invitation.InvitationStatusId == InvitationStatusIds.Read
                ? invitation.UpdatedDate
                : null;
            item.DeclinedDate = invitation.InvitationStatusId == InvitationStatusIds.Rejected
                ? invitation.UpdatedDate
                : null;
            item.ExpiredDate = invitation.InvitationStatusId == InvitationStatusIds.Closed ||
                               invitation.InvitationStatusId == InvitationStatusIds.Expired
                ? invitation.UpdatedDate
                : null;
            item.LastActivityDate = invitation.UpdatedDate;
            item.HasAttachments = invitation.Attachments.Any();
        }

        var result = new PaginatedResult<JobInvitationSummaryDetailsRowDto>(
            items,
            pagedInvitations.Metadata.TotalCount,
            pagedInvitations.Metadata.CurrentPage,
            pagedInvitations.Metadata.PageSize);

        return Result.Ok(result);
    }
}
