using Cortex.Mediator.Queries;
using FluentResults;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Common.Models;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Extensions;
using Tawtheef.Application.Features.Operations.Employee.JobInvitationSummaryDetails.DTOs;
using Tawtheef.Application.Features.Operations.Employee.JobInvitationSummaryDetails.Queries;
using Tawtheef.Domain.Entities.Recruitment;

namespace Tawtheef.Application.Features.Operations.Employee.JobInvitationSummaryDetails.Handlers.Queries;

public sealed class GetJobInvitationSummaryDetailsRowsQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ILocalizationService localizationService)
    : IQueryHandler<GetJobInvitationSummaryDetailsRowsQuery, IResult<PaginatedResult<JobInvitationSummaryDetailsRowDto>>>
{
    public async Task<IResult<PaginatedResult<JobInvitationSummaryDetailsRowDto>>> Handle(
        GetJobInvitationSummaryDetailsRowsQuery query,
        CancellationToken cancellationToken)
    {
        var searchTerm = query.Search?.Trim();

        var invitations = unitOfWork.GetEntityRepository<Invitation>().DbSet
            .AsNoTracking()
            .Include(invitation => invitation.InvitationStatus)
            .Include(invitation => invitation.Applicant)!.ThenInclude(applicant => applicant!.Profile)!
            .ThenInclude(profile => profile!.Nationality)
            .Where(invitation => invitation.JobId == query.JobId)
            .WhereIf(query.StatusId.HasValue, invitation => invitation.InvitationStatusId == query.StatusId!.Value)
            .WhereIf(!string.IsNullOrWhiteSpace(searchTerm), invitation =>
                (invitation.Applicant != null &&
                 (invitation.Applicant.FullNameAr.Contains(searchTerm!) ||
                  invitation.Applicant.FullNameEn.Contains(searchTerm!) ||
                  (invitation.Applicant.PhoneNumber != null &&
                   invitation.Applicant.PhoneNumber.Contains(searchTerm!)))) ||
                (invitation.Applicant != null &&
                 invitation.Applicant.Profile != null &&
                 invitation.Applicant.Profile.NationalNumber != null &&
                 invitation.Applicant.Profile.NationalNumber.Contains(searchTerm!)));

        var pagedInvitations = await invitations.ToPaginationListAsync(query, cancellationToken);
        var items = mapper.Map<List<JobInvitationSummaryDetailsRowDto>>(pagedInvitations.Items);

        for (var index = 0; index < items.Count; index++)
        {
            var invitation = pagedInvitations.Items[index];
            var item = items[index];
            item.FullName = localizationService.GetLocalizedFullName(invitation.Applicant);
            item.Nationality = localizationService.GetLocalizedName(invitation.Applicant?.Profile?.Nationality);
            item.Status = invitation.InvitationStatus == null
                ? new DropdownOptions()
                : new DropdownOptions
                {
                    Id = invitation.InvitationStatus.Id,
                    BackendName = invitation.InvitationStatus.BackendName,
                    Name = localizationService.GetLocalizedName(invitation.InvitationStatus),
                    Description = localizationService.GetLocalizedDescription(invitation.InvitationStatus),
                    AdditionalData = invitation.InvitationStatus.DisplayOrder
                };
        }

        var result = new PaginatedResult<JobInvitationSummaryDetailsRowDto>(
            items,
            pagedInvitations.Metadata.TotalCount,
            pagedInvitations.Metadata.CurrentPage,
            pagedInvitations.Metadata.PageSize);

        return Result.Ok(result);
    }
}
