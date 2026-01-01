using FluentResults;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Extensions;
using Tawtheef.Application.Features.Operations.Employee.JobInvitationSummaryDetails.DTOs;
using Tawtheef.Application.Features.Operations.Employee.JobInvitationSummaryDetails.Queries;
using Tawtheef.Domain.Entities.Recruitment;

namespace Tawtheef.Application.Features.Operations.Employee.JobInvitationSummaryDetails.Handlers.Queries;

public sealed class GetJobInvitationSummaryDetailsRowsQueryHandler(
    IUnitOfWork unitOfWork)
    : IRequestHandler<GetJobInvitationSummaryDetailsRowsQuery, IResult<PaginatedResult<JobInvitationSummaryDetailsRowDto>>>
{
    public async Task<IResult<PaginatedResult<JobInvitationSummaryDetailsRowDto>>> Handle(
        GetJobInvitationSummaryDetailsRowsQuery query,
        CancellationToken cancellationToken)
    {
        var searchTerm = query.Search?.Trim();

        var invitations = unitOfWork.GetEntityRepository<Invitation>().DbSet
            .AsNoTracking()
            .Include(invitation => invitation.InvitationStatus)
            .Include(invitation => invitation.Applicant).ThenInclude(applicant => applicant!.Profile)
            .ThenInclude(profile => profile!.Nationality)
            .Where(invitation => invitation.JobId == query.JobId)
            .WhereIf(query.StatusId.HasValue, invitation => query.StatusId != null && invitation.InvitationStatusId == query.StatusId.Value)
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

        var result = await invitations
            .ProjectToType<JobInvitationSummaryDetailsRowDto>()
            .ToPaginationListAsync(query, cancellationToken);
        return Result.Ok(result);
    }
}
