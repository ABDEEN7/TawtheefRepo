using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Validations;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Applicant;
using Tawtheef.Domain.Entities.Recruitment;

namespace Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals;

internal static class QualificationUniversityReviewGuard
{
    public static async Task<Result> ValidateReviewItemAsync(IUnitOfWork uow, ReviewItem item, CancellationToken ct)
    {
        if (item.Section != ProfileSection.Qualifications ||
            item.TargetType != ReviewTargetType.Row ||
            item.EntityName != ProfileReviewConstants.EntityNames.Qualification)
            return Result.Ok();

        if (item.ProfileChange is not null)
        {
            var snapshot = PendingQualificationSnapshotParser.Deserialize(item.ProfileChange.NewValue);
            if (snapshot is null || !snapshot.DegreeId.HasValue)
                return Result.Fail(ErrorsCodes.InvalidDegreeUniversityId);

            return ValidateApproval(snapshot.DegreeId.Value, snapshot.UniversityId);
        }

        if (!item.EntityId.HasValue)
            return Result.Fail(ErrorsCodes.TargetEntityNotFound);

        var qualification = await uow.GetEntityRepository<Qualification>().DbSet
            .AsNoTracking()
            .Where(q => q.Id == item.EntityId.Value && !q.IsDeleted)
            .Select(q => new { q.DegreeId, q.UniversityId })
            .FirstOrDefaultAsync(ct);
        if (qualification is null)
            return Result.Fail(ErrorsCodes.TargetEntityNotFound);

        return ValidateApproval(qualification.DegreeId, qualification.UniversityId);
    }

    public static async Task<Result> ValidatePersistedProfileAsync(IUnitOfWork uow, Guid profileId, CancellationToken ct)
    {
        var qualifications = await uow.GetEntityRepository<Qualification>().DbSet
            .AsNoTracking()
            .Where(q => q.UserProfileId == profileId && !q.IsDeleted)
            .Select(q => new { q.DegreeId, q.UniversityId })
            .ToListAsync(ct);

        foreach (var qualification in qualifications)
        {
            var result = ValidateApproval(qualification.DegreeId, qualification.UniversityId);
            if (result.IsFailed)
                return result;
        }

        return Result.Ok();
    }

    private static Result ValidateApproval(Guid degreeId, Guid? universityId)
    {
        if (!EducationUniversityRules.RequiresUniversity(degreeId))
            return Result.Ok();

        if (!universityId.HasValue || universityId.Value == Guid.Empty)
            return Result.Fail(ErrorsCodes.InvalidDegreeUniversityId);

        return UniversityIds.IsOther(universityId)
            ? Result.Fail(ErrorsCodes.UniversityResolutionRequired)
            : Result.Ok();
    }
}
