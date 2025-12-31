using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Operations.Admin.Languages.DTOs;
using Tawtheef.Application.Features.Operations.Admin.Languages.Queries;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Application.Features.Operations.Admin.Languages.Handlers.Queries;

public sealed class GetLanguageDetailsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<GetLanguageDetailsQuery, IResult<LanguageAdminDto>>
{
    public async Task<IResult<LanguageAdminDto>> Handle(
        GetLanguageDetailsQuery request,
        CancellationToken cancellationToken)
    {
        var language = await unitOfWork
            .GetEntityRepository<Language>()
            .DbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == request.Id, cancellationToken);

        if (language is null)
            return Result.Fail<LanguageAdminDto>(ErrorsCodes.LanguageNotFound);

        var dto = mapper.Map<LanguageAdminDto>(language);
        return Result.Ok(dto);
    }
}
