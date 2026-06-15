using Application.Recruitment.Features.Profile.DTOs.SaveOperation;
using FluentResults;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Applicant;

namespace Application.Recruitment.Features.Profile.Validators;

internal static class ProfileDuplicateValidation
{
    public static Result ValidateEducation(
        IReadOnlyList<SaveProfileEducationDegreeDto> incoming,
        IReadOnlyList<Qualification> existing)
    {
        return Validate(
            incoming,
            existing,
            DegreeKey.From,
            DegreeKey.From,
            ErrorsCodes.DuplicateEducation);
    }

    public static Result ValidateExperiences(
        IReadOnlyList<ExperienceUpsertDto> incoming,
        IReadOnlyList<Experience> existing)
    {
        return Validate(
            incoming,
            existing,
            ExperienceKey.From,
            ExperienceKey.From,
            ErrorsCodes.DuplicateExperience);
    }

    public static Result ValidateTrainingCourses(
        IReadOnlyList<TrainingCourseUpsertDto> incoming,
        IReadOnlyList<TrainingCourse> existing)
    {
        return Validate(
            incoming,
            existing,
            TrainingCourseKey.From,
            TrainingCourseKey.From,
            ErrorsCodes.DuplicateTrainingCourse);
    }

    public static Result ValidateAchievements(
        IReadOnlyList<AchievementUpsertDto> incoming,
        IReadOnlyList<Achievement> existing)
    {
        return Validate(
            incoming,
            existing,
            AchievementKey.From,
            AchievementKey.From,
            ErrorsCodes.DuplicateAchievement);
    }

    private static Result Validate<TIncoming, TExisting, TKey>(
        IReadOnlyList<TIncoming> incoming,
        IReadOnlyList<TExisting> existing,
        Func<TIncoming, TKey> incomingKey,
        Func<TExisting, TKey> existingKey,
        string errorCode)
        where TIncoming : class
        where TExisting : class
        where TKey : notnull
    {
        if (incoming.GroupBy(incomingKey).Any(group => group.Count() > 1))
            return Result.Fail(errorCode);

        foreach (var dto in incoming)
        {
            var dtoId = GetId(dto);
            var key = incomingKey(dto);
            var hasDuplicateExisting = existing.Any(row =>
                GetId(row) != dtoId &&
                EqualityComparer<TKey>.Default.Equals(existingKey(row), key));

            if (hasDuplicateExisting)
                return Result.Fail(errorCode);
        }

        return Result.Ok();
    }

    private static Guid? GetId(object value)
    {
        return value switch
        {
            SaveProfileEducationDegreeDto dto => NormalizeId(dto.Id),
            ExperienceUpsertDto dto => NormalizeId(dto.Id),
            TrainingCourseUpsertDto dto => NormalizeId(dto.Id),
            AchievementUpsertDto dto => NormalizeId(dto.Id),
            Qualification entity => NormalizeId(entity.Id),
            Experience entity => NormalizeId(entity.Id),
            TrainingCourse entity => NormalizeId(entity.Id),
            Achievement entity => NormalizeId(entity.Id),
            _ => null
        };
    }

    private static Guid? NormalizeId(Guid? id)
    {
        return id is null || id == Guid.Empty ? null : id;
    }

    private static string Normalize(string? value)
    {
        return (value ?? string.Empty).Trim().ToUpperInvariant();
    }

    private sealed record DegreeKey(
        Guid DegreeId,
        Guid CountryId,
        Guid? UniversityId,
        Guid? MajorId,
        Guid? SubMajorId,
        Guid? StudyTypeId,
        Guid? GradeId,
        int? GraduationYear,
        decimal? Gpa)
    {
        public static DegreeKey From(SaveProfileEducationDegreeDto dto) => new(
            dto.DegreeId,
            dto.GradCountryId,
            NormalizeGuid(dto.UniversityId),
            NormalizeGuid(dto.MajorId),
            NormalizeGuid(dto.SubMajorId),
            NormalizeGuid(dto.StudyTypeId),
            NormalizeGuid(dto.GradeId),
            dto.GradYear,
            dto.Gpa);

        public static DegreeKey From(Qualification entity) => new(
            entity.DegreeId,
            entity.CountryId,
            NormalizeGuid(entity.UniversityId),
            NormalizeGuid(entity.MajorId),
            NormalizeGuid(entity.SubMajorId),
            NormalizeGuid(entity.StudyTypeId),
            NormalizeGuid(entity.RatingId),
            entity.GraduationYear,
            entity.GPA);
    }

    private sealed record ExperienceKey(
        string EmployerName,
        string JobTitle,
        Guid CountryId,
        DateOnly StartDate,
        DateOnly? EndDate,
        Guid? QualificationId)
    {
        public static ExperienceKey From(ExperienceUpsertDto dto) => new(
            Normalize(dto.EmployerName),
            Normalize(dto.JobTitle),
            dto.CountryId,
            dto.StartDate,
            dto.EndDate,
            NormalizeGuid(dto.QualificationId));

        public static ExperienceKey From(Experience entity) => new(
            Normalize(entity.EmployerName),
            Normalize(entity.JobTitle),
            entity.CountryId,
            entity.StartDate,
            entity.EndDate,
            NormalizeGuid(entity.QualificationId));
    }

    private sealed record TrainingCourseKey(
        string Provider,
        string Title,
        Guid CountryId,
        DateOnly StartDate,
        DateOnly? EndDate)
    {
        public static TrainingCourseKey From(TrainingCourseUpsertDto dto) => new(
            Normalize(dto.Provider),
            Normalize(dto.Title),
            dto.CountryId,
            dto.StartDate,
            dto.EndDate);

        public static TrainingCourseKey From(TrainingCourse entity) => new(
            Normalize(entity.Provider),
            Normalize(entity.Title),
            entity.CountryId,
            entity.StartDate,
            entity.EndDate);
    }

    private sealed record AchievementKey(
        Guid AchievementTypeId,
        string Title,
        string IssuingAuthority,
        Guid CountryId,
        DateOnly? IssueDate)
    {
        public static AchievementKey From(AchievementUpsertDto dto) => new(
            dto.AchievementTypeId,
            Normalize(dto.Title),
            Normalize(dto.IssuingAuthority),
            dto.CountryId,
            dto.IssueDate);

        public static AchievementKey From(Achievement entity) => new(
            entity.AchievementTypeId,
            Normalize(entity.Title),
            Normalize(entity.IssuingAuthority),
            entity.CountryId,
            entity.IssueDate);
    }

    private static Guid? NormalizeGuid(Guid? id)
    {
        return id is null || id == Guid.Empty ? null : id;
    }
}
