using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Common.Models;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Features.Operations.Employee.JobInvitationSummaryDetails.DTOs;
using Tawtheef.Application.Features.Operations.Employee.JobInvitationSummaryDetails.Queries;
using Tawtheef.Domain.Entities.Recruitment;

namespace Tawtheef.Application.Features.Operations.Employee.JobInvitationSummaryDetails.Handlers.Queries;

public sealed class GetJobInvitationSummaryDetailsRowsQueryHandler(
    IUnitOfWork unitOfWork,
    ILocalizationService localizationService)
    : IRequestHandler<GetJobInvitationSummaryDetailsRowsQuery, IResult<PaginatedResult<JobInvitationSummaryDetailsRowDto>>>
{
    public async Task<IResult<PaginatedResult<JobInvitationSummaryDetailsRowDto>>> Handle(
        GetJobInvitationSummaryDetailsRowsQuery query,
        CancellationToken cancellationToken)
    {
        var invitations = unitOfWork.GetEntityRepository<Invitation>().DbSet
            .AsNoTracking()
            .Include(invitation => invitation.InvitationStatus)
            .Include(invitation => invitation.Applicant).ThenInclude(applicant => applicant!.Profile)
            .ThenInclude(profile => profile!.Nationality)
            .Where(invitation => invitation.JobId == query.JobId);

        if (!string.IsNullOrWhiteSpace(query.AcademicYear) &&
            int.TryParse(query.AcademicYear, out var year))
        {
            invitations = invitations.Where(invitation => invitation.CreatedDate.Year == year);
        }

        if (query.StatusId.HasValue)
        {
            invitations = invitations.Where(invitation => invitation.InvitationStatusId == query.StatusId.Value);
        }

        var searchTerm = query.Search?.Trim();
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            invitations = invitations.Where(invitation =>
                (invitation.Applicant != null &&
                 (invitation.Applicant.FullNameAr.Contains(searchTerm) ||
                  invitation.Applicant.FullNameEn.Contains(searchTerm) ||
                  (invitation.Applicant.PhoneNumber != null &&
                   invitation.Applicant.PhoneNumber.Contains(searchTerm)))) ||
                (invitation.Applicant != null &&
                 invitation.Applicant.Profile != null &&
                 invitation.Applicant.Profile.NationalNumber != null &&
                 invitation.Applicant.Profile.NationalNumber.Contains(searchTerm)));
        }

        var totalCount = await invitations.CountAsync(cancellationToken);

        var ordered = query.SortDirection == "desc"
            ? invitations.OrderByDescending(invitation => invitation.CreatedDate)
            : invitations.OrderBy(invitation => invitation.CreatedDate);

        var invitationList = await ordered
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        var items = invitationList.Select(invitation => new JobInvitationSummaryDetailsRowDto
            {
                InviteId = invitation.Id,
                FullName = localizationService.GetLocalizedFullName(invitation.Applicant),
                Nationality = localizationService.GetLocalizedName(invitation.Applicant?.Profile?.Nationality),
                Phone = invitation.Applicant?.PhoneNumber ?? string.Empty,
                Status = invitation.InvitationStatus == null
                    ? new DropdownOptions()
                    : new DropdownOptions
                    {
                        Id = invitation.InvitationStatus.Id,
                        BackendName = invitation.InvitationStatus.BackendName,
                        Name = localizationService.GetLocalizedName(invitation.InvitationStatus),
                        Description = localizationService.GetLocalizedDescription(invitation.InvitationStatus),
                        AdditionalData = invitation.InvitationStatus.DisplayOrder
                    },
                SentDate = invitation.CreatedDate
            })
            .ToList();

        var result = new PaginatedResult<JobInvitationSummaryDetailsRowDto>(
            items,
            totalCount,
            query.PageNumber,
            query.PageSize);

        return Result.Ok(result);
    }
}
