using Application.Operation.Features.Admin.JobTitles.DTOs;
using Application.Operation.Features.Admin.JobTitles.Queries;
using MediatR;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Application.Operation.Features.Admin.JobTitles.Handlers.Queries;

public sealed class GetListJobTitlesQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetListJobTitlesQuery, IResult<List<JobTitleAdminDto>>>
{
    public async Task<IResult<List<JobTitleAdminDto>>> Handle(
        GetListJobTitlesQuery request,
        CancellationToken cancellationToken)
    {
        var data = await unitOfWork
            .GetEntityRepository<JobTitle>()
            .DbSet
            .AsNoTracking()
            .OrderBy(x => x.JobNameEn)
            .Select(x => new JobTitleAdminDto(
                x.Id,
                x.JobNumber,
                x.JobNameAr,
                x.JobNameEn,
                x.IsActive))
            .ToListAsync(cancellationToken);

        return Result.Ok(data);
    }
}

