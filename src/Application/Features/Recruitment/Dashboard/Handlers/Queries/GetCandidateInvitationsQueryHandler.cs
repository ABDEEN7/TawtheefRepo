using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Extensions;
using Tawtheef.Application.Features.Recruitment.Dashboard.DTOs;
using Tawtheef.Application.Features.Recruitment.Dashboard.Queries;
using Tawtheef.Domain.Entities.Recruitment;

namespace Tawtheef.Application.Features.Recruitment.Dashboard.Handlers.Queries;

public sealed class GetCandidateInvitationsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<GetCandidateInvitationsQuery, IResult<PaginatedResult<CandidateInvitationsDto>>>
{
    public async Task<IResult<PaginatedResult<CandidateInvitationsDto>>> Handle(GetCandidateInvitationsQuery query, CancellationToken cancellationToken)
    {
        var invitations =  await unitOfWork.GetEntityRepository<Invitation>().DbSet
            .AsNoTracking()
            .Include(i => i.InvitationStatus)
            .Include(i => i.Job).ThenInclude(j => j!.JobCategory)
            .Include(i => i.Job).ThenInclude(j => j!.RequestingDepartment)
            .WhereIf(query.InvitationStatusId is not null, i => i.InvitationStatusId == query.InvitationStatusId)
            .WhereIf(query.JobCategoryId is not null, i => i.Job!.JobCategoryId == query.JobCategoryId)
            .WhereIf(query.DepartmentId is not null, i => i.Job!.RequestingDepartmentId == query.DepartmentId)
            .WhereIf(!string.IsNullOrWhiteSpace(query.JobTitle), i => i.Job!.Title.Contains(query.JobTitle!))
            .ToPaginatedListAsync<Invitation, CandidateInvitationsDto>(mapper, query, cancellationToken);

        return Result.Ok(invitations);
    }
}
