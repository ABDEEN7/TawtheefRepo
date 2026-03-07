using Application.Operation.Features.Admin.Languages.DTOs;
using Application.Operation.Features.Admin.Languages.Queries;
using MediatR;
using FluentResults;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;

namespace Application.Operation.Features.Admin.Languages.Handlers.Queries;

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

