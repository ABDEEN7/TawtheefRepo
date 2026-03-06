using Application.Recruitment.Features.Dashboard.DTOs;
using Application.Recruitment.Features.Dashboard.Queries;
using MediatR;
using FluentResults;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Extensions;
using Tawtheef.Domain.Entities.Recruitment;

namespace Application.Recruitment.Features.Dashboard.Handlers.Queries;

public sealed class GetCandidateInvitationsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<GetCandidateInvitationsQuery, IResult<PaginatedResult<CandidateInvitationsDto>>>
{
    public async Task<IResult<PaginatedResult<CandidateInvitationsDto>>> Handle(GetCandidateInvitationsQuery query, CancellationToken cancellationToken)
    {
        var invitations = await unitOfWork.GetEntityRepository<Invitation>().DbSet
            .AsNoTracking()
            .Include(i => i.InvitationStatus)
            .Include(i => i.Job).ThenInclude(j => j!.JobCategory)
            .Include(i => i.Job).ThenInclude(j => j!.Department)
            .Include(i => i.Job).ThenInclude(j => j!.JobTitle)
            .WhereIf(query.UserId != Guid.Empty , i => i.ApplicantId == query.UserId) // <-- add this
            .WhereIf(query.InvitationStatusId is not null, i => i.InvitationStatusId == query.InvitationStatusId)
            .WhereIf(query.JobCategoryId is not null, i => i.Job!.JobCategoryId == query.JobCategoryId)
            .WhereIf(query.DepartmentId is not null, i => i.Job!.DepartmentId == query.DepartmentId)
            .WhereIf(!string.IsNullOrWhiteSpace(query.JobTitle),
                i => i.Job!.JobTitle!.JobNameAr.Contains(query.JobTitle!) || i.Job!.JobTitle!.JobNameEn.Contains(query.JobTitle!))
            .ToPaginatedListAsync<Invitation, CandidateInvitationsDto>(mapper, query, cancellationToken);

        return Result.Ok(invitations);
    }
}

