using System.Text.Json;
using FluentResults;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Common.Mappers;
using Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileApprovals.DTOs;
using Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileApprovals.Queries;
using Tawtheef.Application.Features.Recruitment.Profile;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities;
using Tawtheef.Domain.Entities.Applicant;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileApprovals.Handlers.Queries;

public sealed class GetProfilePartialChangesHandler(
        IUnitOfWork uow,
        IMapper mapper,
        IMediaUrlResolver media,
        ILocalizationService localization)
    : IRequestHandler<GetProfilePartialChangesQuery, Result<GetProfilePartialChangesDetailDto>>
{
    private sealed record FileDisplay(string FileName, string Url);

    private static readonly HashSet<string> ResourceFieldNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "ResumeAttachmentId",
        "NationalCardId",
        "BirthCertificateId",
        "MarriageCertificateId",
        "SponsorCardResourceId",
        "SponsorCardId",
        "NationalAddressCertificateId",
        "ResidenceAddressCertificateId",
        "CertificateId",
        "AttachmentResourceId",
        "AttachmentId"
    };

    public async Task<Result<GetProfilePartialChangesDetailDto>> Handle(GetProfilePartialChangesQuery request, CancellationToken ct)
    {
        var profile = await UserProfileLoader.GetFullProfile(uow, request.UserProfileId, ct);
        if (profile is null)
            return Result.Fail<GetProfilePartialChangesDetailDto>(ErrorsCodes.UserProfileNotFound);

        var assignmentRepo = uow.GetEntityRepository<ProfileAssignment>();
        var isAssigned = await assignmentRepo.DbSet
            .AsNoTracking()
            .AnyAsync(a => a.UserProfileId == profile.Id && a.EmployeeId == request.OfficerId && a.IsActive, ct);

        if (!isAssigned)
            return Result.Fail<GetProfilePartialChangesDetailDto>(ErrorsCodes.UnauthorizedAction);

        var reviewRepo = uow.GetEntityRepository<ReviewItem>();
        var reviewItems = await reviewRepo.DbSet
            .AsNoTracking()
            .Where(r => r.UserProfileId == profile.Id
                        && r.ProfileChangeId != null
                        && r.Status != ReviewStatus.Approved)
            .Include(r => r.ProfileChange)
            .OrderByDescending(r => r.UpdatedDate ?? r.CreatedDate)
            .ToListAsync(ct);

        var latestItems = reviewItems
            .GroupBy(r => new { r.TargetType, r.Section, r.FieldPath, r.EntityName, r.EntityId, r.ResourceId, r.ProfileChangeId })
            .Select(g => g.First())
            .ToList();

        var enrichment = await BuildEnrichmentAsync(latestItems, profile, ct);

        var resourceRepo = uow.GetEntityRepository<Resource>();
        var resources = enrichment.ResourceIds.Count == 0
            ? new Dictionary<Guid, FileDisplay>()
            : await resourceRepo.DbSet
                .AsNoTracking()
                .Where(r => enrichment.ResourceIds.Contains(r.Id))
                .ToDictionaryAsync(r => r.Id, r => new FileDisplay(r.Name, media.ResolveAbsolute(r.Url)), ct);

        var mapped = latestItems
            .Select(item =>
            {
                var dto = mapper.Map<ProfileApprovalItemDto>(item);

                var oldDisplay = EnrichJson(item.ProfileChange?.OldValue, enrichment, resources);
                var newDisplay = EnrichJson(item.ProfileChange?.NewValue, enrichment, resources);
                dto = dto with { OldValue = oldDisplay, NewValue = newDisplay };

                if (item.ResourceId.HasValue && resources.TryGetValue(item.ResourceId.Value, out var resource))
                {
                    dto.ResourceUrl = resource.Url;
                }

                return (Item: item, Dto: dto);
            })
            .ToList();

        var sections = mapped
            .GroupBy(x => x.Item.Section)
            .Select(group =>
            {
                var items = group
                    .Select(x => x.Dto)
                    .OrderBy(x => (int)x.TargetType)
                    .ThenBy(x => x.Title)
                    .ToList();

                var status = ResolveSectionStatus(items);
                var reviewedAt = group
                    .Where(x => x.Item.ReviewedAtUtc.HasValue)
                    .OrderByDescending(x => x.Item.ReviewedAtUtc)
                    .Select(x => x.Item.ReviewedAtUtc)
                    .FirstOrDefault();

                var note = items
                    .Select(i => i.Note)
                    .FirstOrDefault(n => !string.IsNullOrWhiteSpace(n));

                return new ProfileApprovalSectionDto
                {
                    Section = group.Key,
                    Status = status,
                    Note = note,
                    ReviewedAtUtc = reviewedAt,
                    Items = items,
                    HasAttachments = items.Any(e => e.TargetType == ReviewTargetType.Attachment)
                };
            })
            .OrderBy(s => (int)s.Section)
            .ToList();

        using var scope = new MapContextScope();
        scope.Context.Parameters[ResourceMapper.MediaKey] = media;

        var profileData = mapper.Map<ProfileApprovalDataDto>(profile);
        profileData.Qualifications = profile.Qualifications?
            .Select(mapper.Map<QualificationDto>)
            .ToList() ?? [];

        var dto = new GetProfilePartialChangesDetailDto
        {
            UserProfileId = profile.Id,
            UserId = profile.UserId,
            FullName = profile.User?.FullNameEn ?? profile.User?.FullNameAr ?? string.Empty,
            CandidateType = localization.GetLocalizedName(profile.CandidateType),
            TargetEntity = localization.GetLocalizedName(profile.TargetEntity),
            Profile = profileData,
            Sections = sections
        };

        var auditRepo = uow.GetEntityRepository<AuditTrailEntry>();
        await auditRepo.AddAsync(new AuditTrailEntry
        {
            UserProfileId = profile.Id,
            UserId = request.OfficerId,
            ActionType = "OpenProfileChangeReview",
            Notes = "Profile opened for change requests review",
            Section = nameof(ProfileSection.Personal)
        });
        await uow.SaveChangesAsync(ct);

        return Result.Ok(dto);
    }

    private sealed class EnrichmentMaps
    {
        public HashSet<Guid> ResourceIds { get; } = [];
        public Dictionary<Guid, string> CandidateTypeNames { get; } = new();
        public Dictionary<Guid, string> TargetEntityNames { get; } = new();
        public Dictionary<Guid, string> OfficeNames { get; } = new();
        public Dictionary<Guid, string> CountryNames { get; } = new();
        public Dictionary<Guid, string> GenderNames { get; } = new();
        public Dictionary<Guid, string> ReligionNames { get; } = new();
        public Dictionary<Guid, string> MaritalStatusNames { get; } = new();
        public Dictionary<Guid, string> SponsorTypeNames { get; } = new();

        public Dictionary<Guid, string> DegreeNames { get; } = new();
        public Dictionary<Guid, string> UniversityNames { get; } = new();
        public Dictionary<Guid, string> MajorNames { get; } = new();
        public Dictionary<Guid, string> StudyTypeNames { get; } = new();
        public Dictionary<Guid, string> RatingGradeNames { get; } = new();
        public Dictionary<Guid, string> AchievementTypeNames { get; } = new();
        public Dictionary<Guid, string> SkillNames { get; } = new();
        public Dictionary<Guid, string> SkillLevelNames { get; } = new();
        public Dictionary<Guid, string> LanguageNames { get; } = new();
        public Dictionary<Guid, string> LanguageLevelNames { get; } = new();

        public Dictionary<Guid, string> QualificationLabels { get; } = new();
    }

    private async Task<EnrichmentMaps> BuildEnrichmentAsync(IReadOnlyList<ReviewItem> items, UserProfile profile, CancellationToken ct)
    {
        var maps = new EnrichmentMaps();

        var candidateTypeIds = new HashSet<Guid>();
        var targetEntityIds = new HashSet<Guid>();
        var officeIds = new HashSet<Guid>();
        var countryIds = new HashSet<Guid>();
        var genderIds = new HashSet<Guid>();
        var religionIds = new HashSet<Guid>();
        var maritalStatusIds = new HashSet<Guid>();
        var sponsorTypeIds = new HashSet<Guid>();

        var degreeIds = new HashSet<Guid>();
        var universityIds = new HashSet<Guid>();
        var majorIds = new HashSet<Guid>();
        var studyTypeIds = new HashSet<Guid>();
        var ratingGradeIds = new HashSet<Guid>();
        var achievementTypeIds = new HashSet<Guid>();
        var skillIds = new HashSet<Guid>();
        var skillLevelIds = new HashSet<Guid>();
        var languageIds = new HashSet<Guid>();
        var languageLevelIds = new HashSet<Guid>();
        var qualificationIds = new HashSet<Guid>();

        foreach (var item in items)
        {
            if (item.ResourceId.HasValue)
                maps.ResourceIds.Add(item.ResourceId.Value);

            CollectIds(item.ProfileChange?.OldValue);
            CollectIds(item.ProfileChange?.NewValue);
        }

        await FillLookupAsync<CandidateType>(candidateTypeIds, maps.CandidateTypeNames, ct);
        await FillLookupAsync<TargetEntity>(targetEntityIds, maps.TargetEntityNames, ct);
        await FillLookupAsync<Office>(officeIds, maps.OfficeNames, ct);
        await FillLookupAsync<Country>(countryIds, maps.CountryNames, ct);
        await FillLookupAsync<Gender>(genderIds, maps.GenderNames, ct);
        await FillLookupAsync<Religion>(religionIds, maps.ReligionNames, ct);
        await FillLookupAsync<MaritalStatus>(maritalStatusIds, maps.MaritalStatusNames, ct);
        await FillLookupAsync<SponsorType>(sponsorTypeIds, maps.SponsorTypeNames, ct);

        await FillLookupAsync<Degree>(degreeIds, maps.DegreeNames, ct);
        await FillLookupAsync<University>(universityIds, maps.UniversityNames, ct);
        await FillLookupAsync<Major>(majorIds, maps.MajorNames, ct);
        await FillLookupAsync<StudyType>(studyTypeIds, maps.StudyTypeNames, ct);
        await FillLookupAsync<RatingGrade>(ratingGradeIds, maps.RatingGradeNames, ct);
        await FillLookupAsync<AchievementType>(achievementTypeIds, maps.AchievementTypeNames, ct);
        await FillLookupAsync<Skill>(skillIds, maps.SkillNames, ct);
        await FillLookupAsync<SkillLevel>(skillLevelIds, maps.SkillLevelNames, ct);
        await FillLookupAsync<Language>(languageIds, maps.LanguageNames, ct);
        await FillLookupAsync<LanguageLevel>(languageLevelIds, maps.LanguageLevelNames, ct);

        maps.QualificationLabels.Clear();
        foreach (var q in profile.Qualifications ?? [])
        {
            var label = BuildQualificationLabel(q);
            if (!string.IsNullOrWhiteSpace(label))
            {
                maps.QualificationLabels[q.Id] = label;
            }
        }

        // Add any qualification ids referenced by changes (for mapping to the label above).
        // If the qualification is missing (deleted), it will be shown as raw id.
        _ = qualificationIds;

        return maps;

        void CollectIds(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return;

            try
            {
                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.ValueKind != JsonValueKind.Object)
                    return;

                foreach (var prop in doc.RootElement.EnumerateObject())
                {
                    if (prop.Value.ValueKind != JsonValueKind.String)
                        continue;

                    var raw = prop.Value.GetString();
                    if (!Guid.TryParse(raw, out var id) || id == Guid.Empty)
                        continue;

                    if (ResourceFieldNames.Contains(prop.Name))
                    {
                        maps.ResourceIds.Add(id);
                        continue;
                    }

                    switch (prop.Name)
                    {
                        case "CandidateTypeId":
                            candidateTypeIds.Add(id);
                            break;
                        case "TargetEntityId":
                            targetEntityIds.Add(id);
                            break;
                        case "OfficeId":
                            officeIds.Add(id);
                            break;

                        case "NationalityId":
                        case "ResidenceCountryId":
                        case "InterviewLocationId":
                        case "GradCountryId":
                        case "CountryId":
                            countryIds.Add(id);
                            break;

                        case "GenderId":
                            genderIds.Add(id);
                            break;
                        case "ReligionId":
                            religionIds.Add(id);
                            break;
                        case "MaritalStatusId":
                            maritalStatusIds.Add(id);
                            break;
                        case "SponsorTypeId":
                            sponsorTypeIds.Add(id);
                            break;

                        case "DegreeId":
                            degreeIds.Add(id);
                            break;
                        case "UniversityId":
                            universityIds.Add(id);
                            break;
                        case "MajorId":
                        case "SubMajorId":
                            majorIds.Add(id);
                            break;
                        case "StudyTypeId":
                            studyTypeIds.Add(id);
                            break;
                        case "GradeId":
                            ratingGradeIds.Add(id);
                            break;
                        case "AchievementTypeId":
                            achievementTypeIds.Add(id);
                            break;

                        case "SkillId":
                            skillIds.Add(id);
                            break;
                        case "LevelId":
                            skillLevelIds.Add(id);
                            break;
                        case "LanguageId":
                            languageIds.Add(id);
                            break;
                        case "SpeakingLevelId":
                        case "WritingLevelId":
                        case "ReadingLevelId":
                            languageLevelIds.Add(id);
                            break;

                        case "QualificationId":
                            qualificationIds.Add(id);
                            break;
                    }
                }
            }
            catch (JsonException)
            {
                // Ignore invalid JSON; UI will show raw strings.
            }
        }
    }

    private async Task FillLookupAsync<TLookup>(HashSet<Guid> ids, Dictionary<Guid, string> target, CancellationToken ct)
        where TLookup : LookupBase
    {
        target.Clear();
        if (ids.Count == 0)
            return;

        var repo = uow.GetEntityRepository<TLookup>();
        var items = await repo.DbSet
            .AsNoTracking()
            .Where(l => ids.Contains(l.Id))
            .ToListAsync(ct);

        foreach (var item in items)
        {
            target[item.Id] = localization.GetLocalizedName(item);
        }
    }

    private static string? EnrichJson(string? json, EnrichmentMaps maps, IReadOnlyDictionary<Guid, FileDisplay> files)
    {
        if (string.IsNullOrWhiteSpace(json))
            return json;

        try
        {
            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.ValueKind != JsonValueKind.Object)
                return json;

            var enriched = new Dictionary<string, object?>(StringComparer.Ordinal);
            foreach (var prop in doc.RootElement.EnumerateObject())
            {
                enriched[prop.Name] = ConvertValue(prop.Name, prop.Value, maps, files);
            }

            return JsonSerializer.Serialize(enriched);
        }
        catch (JsonException)
        {
            return json;
        }
    }

    private static object? ConvertValue(
        string fieldName,
        JsonElement value,
        EnrichmentMaps maps,
        IReadOnlyDictionary<Guid, FileDisplay> files)
    {
        if (value.ValueKind == JsonValueKind.Null)
            return null;

        if (value.ValueKind == JsonValueKind.String)
        {
            var raw = value.GetString();
            if (raw is null)
                return null;

            if (!Guid.TryParse(raw, out var id) || id == Guid.Empty)
                return raw;

            if (ResourceFieldNames.Contains(fieldName))
            {
                return files.TryGetValue(id, out var file)
                    ? new Dictionary<string, object?>
                    {
                        ["resourceId"] = id,
                        ["fileName"] = file.FileName,
                        ["url"] = file.Url
                    }
                    : raw;
            }

            var name = string.Empty;
            return fieldName switch
            {
                "CandidateTypeId" when maps.CandidateTypeNames.TryGetValue(id, out name)
                    => LookupObj(id, name),
                "TargetEntityId" when maps.TargetEntityNames.TryGetValue(id, out name)
                    => LookupObj(id, name),
                "OfficeId" when maps.OfficeNames.TryGetValue(id, out name)
                    => LookupObj(id, name),

                "NationalityId" when maps.CountryNames.TryGetValue(id, out name)
                    => LookupObj(id, name),
                "ResidenceCountryId" when maps.CountryNames.TryGetValue(id, out name)
                    => LookupObj(id, name),
                "InterviewLocationId" when maps.CountryNames.TryGetValue(id, out name)
                    => LookupObj(id, name),
                "GradCountryId" when maps.CountryNames.TryGetValue(id, out name)
                    => LookupObj(id, name),
                "CountryId" when maps.CountryNames.TryGetValue(id, out name)
                    => LookupObj(id, name),

                "GenderId" when maps.GenderNames.TryGetValue(id, out name)
                    => LookupObj(id, name),
                "ReligionId" when maps.ReligionNames.TryGetValue(id, out name)
                    => LookupObj(id, name),
                "MaritalStatusId" when maps.MaritalStatusNames.TryGetValue(id, out name)
                    => LookupObj(id, name),
                "SponsorTypeId" when maps.SponsorTypeNames.TryGetValue(id, out name)
                    => LookupObj(id, name),

                "DegreeId" when maps.DegreeNames.TryGetValue(id, out name)
                    => LookupObj(id, name),
                "UniversityId" when maps.UniversityNames.TryGetValue(id, out name)
                    => LookupObj(id, name),
                "MajorId" when maps.MajorNames.TryGetValue(id, out name)
                    => LookupObj(id, name),
                "SubMajorId" when maps.MajorNames.TryGetValue(id, out name)
                    => LookupObj(id, name),
                "StudyTypeId" when maps.StudyTypeNames.TryGetValue(id, out name)
                    => LookupObj(id, name),
                "GradeId" when maps.RatingGradeNames.TryGetValue(id, out name)
                    => LookupObj(id, name),
                "AchievementTypeId" when maps.AchievementTypeNames.TryGetValue(id, out name)
                    => LookupObj(id, name),

                "SkillId" when maps.SkillNames.TryGetValue(id, out name)
                    => LookupObj(id, name),
                "LevelId" when maps.SkillLevelNames.TryGetValue(id, out name)
                    => LookupObj(id, name),

                "LanguageId" when maps.LanguageNames.TryGetValue(id, out name)
                    => LookupObj(id, name),
                "SpeakingLevelId" when maps.LanguageLevelNames.TryGetValue(id, out name)
                    => LookupObj(id, name),
                "WritingLevelId" when maps.LanguageLevelNames.TryGetValue(id, out name)
                    => LookupObj(id, name),
                "ReadingLevelId" when maps.LanguageLevelNames.TryGetValue(id, out name)
                    => LookupObj(id, name),

                "QualificationId" when maps.QualificationLabels.TryGetValue(id, out name)
                    => LookupObj(id, name),

                _ => raw
            };
        }

        if (value.ValueKind == JsonValueKind.True || value.ValueKind == JsonValueKind.False)
            return value.GetBoolean();

        if (value.ValueKind == JsonValueKind.Number)
        {
            if (value.TryGetInt64(out var l)) return l;
            if (value.TryGetDecimal(out var d)) return d;
            return value.GetDouble();
        }

        // Nested objects/arrays are kept as-is.
        return value;
    }

    private static Dictionary<string, object?> LookupObj(Guid id, string name)
        => new()
        {
            ["id"] = id,
            ["name"] = name
        };

    private string BuildQualificationLabel(Qualification q)
    {
        var parts = new List<string>();

        if (q.Degree is not null)
            parts.Add(localization.GetLocalizedName(q.Degree));
        if (q.Major is not null)
            parts.Add(localization.GetLocalizedName(q.Major));
        if (q.University is not null)
            parts.Add(localization.GetLocalizedName(q.University));
        if (q.GraduationYear is not null)
            parts.Add(q.GraduationYear.Value.ToString());

        return string.Join(" - ", parts);
    }

    private static ReviewStatus ResolveSectionStatus(IReadOnlyList<ProfileApprovalItemDto> items)
    {
        if (items.Any(i => i.Status == ReviewStatus.NeedsCorrection))
            return ReviewStatus.NeedsCorrection;

        if (items.Any(i => i.Status == ReviewStatus.Rejected))
            return ReviewStatus.Rejected;

        if (items.Any(i => i.Status is ReviewStatus.Pending or ReviewStatus.NotReviewed))
            return ReviewStatus.Pending;

        return items.Count == 0 ? ReviewStatus.Pending : ReviewStatus.Approved;
    }
}
