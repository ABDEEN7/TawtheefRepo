using Application.Operation.Features.Employee.MinisterOffice.DTOs;
using Application.Operation.Features.Employee.MinisterOffice.Queries;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Extensions;
using Tawtheef.Domain.Entities.MinisterOffice;

namespace Application.Operation.Features.Employee.MinisterOffice.Handlers;

public sealed class GetMinisterOfficeCandidateAuditLogQueryHandler(IUnitOfWork uow)
    : IRequestHandler<GetMinisterOfficeCandidateAuditLogQuery,
        IResult<PaginatedResult<MinisterOfficeCandidateAuditLogDto>>>
{
    public async Task<IResult<PaginatedResult<MinisterOfficeCandidateAuditLogDto>>> Handle(
        GetMinisterOfficeCandidateAuditLogQuery request,
        CancellationToken ct)
    {
        var repo = uow.GetEntityRepository<MinisterOfficeCandidateAuditLog>();

        var query = repo.DbSet
            .AsNoTracking()
            .Where(l => l.CandidateId == request.CandidateId)
            .OrderByDescending(l => l.CreatedDate)
            .Select(l => new MinisterOfficeCandidateAuditLogDto
            {
                Id = l.Id,
                Action = l.Action,
                Details = l.Details,
                Qid = l.Qid,
                OperatorNameEn = l.CreatedBy != null ? l.CreatedBy.FullNameEn : "System",
                OperatorNameAr = l.CreatedBy != null ? l.CreatedBy.FullNameAr : "النظام",
                CreatedDate = l.CreatedDate
            });

        var result = await query.ToPaginatedListAsync(request, ct);

        return Result.Ok(result);
    }
}
