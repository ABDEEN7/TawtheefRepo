using Cortex.Mediator.Queries;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models;
using Tawtheef.Application.Extensions;
using Tawtheef.Application.Features.Lookups.Queries;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Tawtheef.Application.Features.Lookups.Handlers.Queries;

public sealed class GetCountriesQueryHandler(IUnitOfWork unitOfWork)
    : IQueryHandler<GetCountriesQuery, IResult<List<DropdownOptions>>>
{
    public async Task<IResult<List<DropdownOptions>>> Handle(GetCountriesQuery request,
        CancellationToken cancellationToken)
    {
        //get language from the header request
        var language = request.Language;
        
        var countries = await unitOfWork.GetEntityRepository<Country>()
            .DbSet
            .Where(s => s.IsActive)
            .Select((c)=> new DropdownOptions {
                Id = c.Id,
                Name = c.GetLocalizedName(language)!,
                Description = c.GetLocalizedDescription(language)!,
                BackendName = c.BackendName,
                AdditionalData = new { c.Code }
            })
            .ToListAsync(cancellationToken);
        return Result.Ok(countries);
    }
}
