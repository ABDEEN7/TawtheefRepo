using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Tawtheef.Application.Common.Validations;

public static class EducationUniversityRules
{
    private static readonly HashSet<Guid> DegreesWithoutUniversity =
    [
        DegreeIds.Preparatory,
        DegreeIds.Primary,
        DegreeIds.Secondary
    ];

    public static bool RequiresUniversity(Guid degreeId) => !DegreesWithoutUniversity.Contains(degreeId);

    public static async Task<Result> ValidateSelectionAsync(
        IUnitOfWork uow,
        Guid degreeId,
        Guid countryId,
        Guid? universityId,
        CancellationToken ct)
    {
        if (!RequiresUniversity(degreeId))
            return Result.Ok();

        if (!universityId.HasValue || universityId.Value == Guid.Empty)
            return Result.Fail(ErrorsCodes.InvalidDegreeUniversityId);

        if (UniversityIds.IsOther(universityId))
            return Result.Ok();

        var university = await uow.GetEntityRepository<University>().DbSet
            .AsNoTracking()
            .Include(u => u.City)
            .FirstOrDefaultAsync(u =>
                u.Id == universityId.Value &&
                u.IsActive &&
                !u.IsDeleted,
                ct);

        if (university?.City?.CountryId != countryId)
            return Result.Fail(ErrorsCodes.InvalidDegreeUniversityId);

        return Result.Ok();
    }
}
