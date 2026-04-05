using Application.Operation.Features.Employee.Kawader.DTOs;
using Application.Operation.Features.Employee.Kawader.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Domain.Entities.Kawader;

namespace Application.Operation.Features.Employee.Kawader.Handlers;

public sealed class GetKawaderUserQueryHandler(IUnitOfWork uow)
    : IRequestHandler<GetKawaderUserQuery, PaginatedResult<KawaderUserDto>>
{
    public async Task<PaginatedResult<KawaderUserDto>> Handle(GetKawaderUserQuery request, CancellationToken cancellationToken)
    {
        var query = uow.GetEntityRepository<KawaderQid>().DbSet.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var search = request.SearchTerm.Trim();
            query = query.Where(x => 
                x.Qid.Contains(search) || 
                (x.FullName != null && x.FullName.Contains(search)) || 
                (x.Email != null && x.Email.Contains(search)) || 
                (x.PhoneNumber != null && x.PhoneNumber.Contains(search))
            );
        }

        query = query.OrderByDescending(x => x.CreatedDate);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new KawaderUserDto
            {
                Qid = x.Qid,
                FullName = x.FullName,
                Email = x.Email,
                PhoneNumber = x.PhoneNumber,
                IsInvited = x.IsInvited,
                InvitedAt = x.InvitedAt,
                CreatedDate = x.CreatedDate
            })
            .ToListAsync(cancellationToken);

        return new PaginatedResult<KawaderUserDto>(items, totalCount, request.PageNumber, request.PageSize);
    }
}
