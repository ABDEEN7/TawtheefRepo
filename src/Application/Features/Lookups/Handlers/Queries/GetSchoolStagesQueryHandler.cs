using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models;
using Tawtheef.Application.Features.Lookups.Queries;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Application.Features.Lookups.Handlers.Queries;

// Not BaseLookupQueryHandler: that one sorts by name, but school stages have a natural order
// (Kindergarten -> Primary -> Preparatory -> Secondary), kept in DisplayOrder. Same DropdownOptions shape.
public sealed class GetSchoolStagesQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetSchoolStagesQuery, IResult<List<DropdownOptions>>>
{
    public async Task<IResult<List<DropdownOptions>>> Handle(GetSchoolStagesQuery request, CancellationToken cancellationToken)
    {
        var isArabic = string.Equals(request.Language?.Trim(), "ar", StringComparison.OrdinalIgnoreCase);

        var stages = await unitOfWork.GetEntityRepository<SchoolStage>().DbSet
            .AsNoTracking()
            .Where(s => s.IsActive)
            .OrderBy(s => s.DisplayOrder)
            .ToListAsync(cancellationToken);

        return Result.Ok(stages
            .Select(s => new DropdownOptions
            {
                Id = s.Id,
                BackendName = s.BackendName,
                Name = isArabic ? s.NameAr : s.NameEn,
                Description = isArabic ? s.DescriptionAr : s.DescriptionEn,
                AdditionalData = new { s.NameAr, s.NameEn, s.DescriptionAr, s.DescriptionEn }
            })
            .ToList());
    }
}
