using CSharpFunctionalExtensions;
using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Recruitment.Dashboard.DTOs;
using Tawtheef.Application.Features.Recruitment.Dashboard.Queries;
using Tawtheef.Domain.Entities.Recruitment;

namespace Tawtheef.Application.Features.Recruitment.Dashboard.Handlers.Queries;

public sealed class GetCandidateInvitationsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<GetCandidateInvitationsQuery, Result<List<CandidateInvitationsDto>>>
{
    public async Task<Result<List<CandidateInvitationsDto>>> Handle(GetCandidateInvitationsQuery query, CancellationToken cancellationToken)
    {
        var dbSet = unitOfWork.GetEntityRepository<Invitation>().DbSet;
        
        // Start base query
        var invitationsQuery =  dbSet
            .AsNoTracking()
            .Include(i => i.InvitationStatus)
            .Include(i => i.Job)
            .ThenInclude(j => j!.JobCategory)
            .Include(i => i.Job)
            .ThenInclude(j => j!.RequestingDepartment)
            .AsQueryable();

        // Apply optional filters
        if (query.InvitationStatusId != null)
            invitationsQuery = invitationsQuery
                .Where(i => i.InvitationStatusId == query.InvitationStatusId);

        if (query.JobCategoryId != null)
            invitationsQuery = invitationsQuery
                .Where(i => i.Job!.JobCategoryId == query.JobCategoryId);

        if (query.DepartmentId != null)
            invitationsQuery = invitationsQuery
                .Where(i => i.Job!.RequestingDepartmentId == query.DepartmentId);

        if (!string.IsNullOrWhiteSpace(query.JobTitle))
            invitationsQuery = invitationsQuery
                .Where(i => i.Job!.Title.Contains(query.JobTitle));

        var result = await invitationsQuery.OrderByDescending(x => x.CreatedDate)
            .ToListAsync(cancellationToken);

        return Result.Success(mapper.Map<List<CandidateInvitationsDto>>(result));
    }
}
