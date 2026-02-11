using Application.Operation.Features.Admin.HomeContent.Faqs.DTOs;
using Application.Operation.Features.Admin.HomeContent.Faqs.Queries;
using Cortex.Mediator.Queries;
using FluentResults;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Content;

namespace Application.Operation.Features.Admin.HomeContent.Faqs.Handlers.Queries;

public sealed class ListFaqsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IQueryHandler<ListFaqsQuery, IResult<IReadOnlyCollection<FAQAdminDto>>>
{
    public async Task<IResult<IReadOnlyCollection<FAQAdminDto>>> Handle(
        ListFaqsQuery request,
        CancellationToken cancellationToken)
    {
        var faqs = await unitOfWork
            .GetEntityRepository<FAQ>()
            .DbSet
            .AsNoTracking()
            .OrderBy(f => f.DisplayOrder)
            .ThenBy(f => f.CreatedDate)
            .ProjectToType<FAQAdminDto>(mapper.Config)
            .ToListAsync(cancellationToken);

        return Result.Ok<IReadOnlyCollection<FAQAdminDto>>(faqs);
    }
}
