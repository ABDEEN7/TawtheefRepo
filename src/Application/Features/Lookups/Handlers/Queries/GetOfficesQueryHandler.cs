using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models;
using Tawtheef.Application.Extensions;
using Tawtheef.Application.Features.Lookups.Queries;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Tawtheef.Application.Features.Lookups.Handlers.Queries;

public sealed class GetOfficesQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetOfficesQuery, IResult<List<DropdownOptions>>>
{
    public async Task<IResult<List<DropdownOptions>>> Handle(GetOfficesQuery request, CancellationToken cancellationToken)
    {
        var language = request.Language!;

        var officesQuery = unitOfWork.GetEntityRepository<Office>()
            .DbSet
            .AsQueryable();

        if (request.CountryId.HasValue)
        {
            officesQuery = officesQuery.Where(o => o.CountryId == request.CountryId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            officesQuery = officesQuery.Search(request.Search!, language);
        }

        var offices = await officesQuery
            .OrderBy(o => o.DisplayOrder)
            .Select(o => new DropdownOptions
            {
                Id = o.Id,
                Name = o.GetLocalizedName(language)!,
                Description = o.GetLocalizedDescription(language)!,
                BackendName = o.BackendName,
                AdditionalData = new { o.Code, o.CountryId }
            })
            .ToListAsync(cancellationToken);

        return Result.Ok(offices);
    }
}
