using Application.Operation.Features.Admin.HomeContent.Faqs.DTOs;
using Application.Operation.Features.Admin.HomeContent.Faqs.Queries;
using Cortex.Mediator.Queries;
using FluentResults;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Content;

namespace Application.Operation.Features.Admin.HomeContent.Faqs.Handlers.Queries;

public sealed class GetFaqDetailsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IQueryHandler<GetFaqDetailsQuery, IResult<FAQAdminDto>>
{
    public async Task<IResult<FAQAdminDto>> Handle(
        GetFaqDetailsQuery request,
        CancellationToken cancellationToken)
    {
        var faq = await unitOfWork
            .GetEntityRepository<FAQ>()
            .DbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(f => f.Id == request.FaqId, cancellationToken);

        if (faq is null)
            return Result.Fail<FAQAdminDto>(ErrorsCodes.NotFound);

        return Result.Ok(mapper.Map<FAQAdminDto>(faq));
    }
}
