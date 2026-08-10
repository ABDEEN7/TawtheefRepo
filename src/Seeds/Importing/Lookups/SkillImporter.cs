using System.Globalization;
using CsvHelper;
using Microsoft.EntityFrameworkCore;
using Seeds.Models;
using Seeds.SeedData;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;
using Tawtheef.Domain.Entities.Users;

namespace Seeds.Importing.Lookups;

internal static class SkillImporter
{
    private const string FileName = "SkillData.csv";

    public static Task ImportAsync(
        SeedImportContext context,
        CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(context);

        var path = SeedDataPath.Get(FileName);

        using var reader = new StreamReader(path);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        var createdById = AdminUserIds.Admin1UserId;
        var now = DateTime.UtcNow;

        var skillTypeByBackend =
            context.Db.SkillType
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .ToDictionary(
                    x => x.BackendName,
                    x => x.Id,
                    StringComparer.OrdinalIgnoreCase);

        var existingIds =
            new HashSet<Guid>(
                context.Db.Skill
                    .AsNoTracking()
                    .Select(x => x.Id));

        var existingBackends =
            new HashSet<string>(
                context.Db.Skill
                    .AsNoTracking()
                    .Select(x => x.BackendName),
                StringComparer.OrdinalIgnoreCase);

        while (csv.Read())
        {
            ct.ThrowIfCancellationRequested();

            var row = csv.Context.Parser?.Row;

            try
            {
                dynamic record =
                    csv.GetRecord<dynamic>();

                var id =
                    Guid.Parse(
                        (string)record.Id);

                var skillTypeBackendName =
                    ((string)record.SkillTypeBackendName)
                    .Trim();

                var backendName =
                    ((string)record.BackendName)
                    .Trim();

                var nameAr =
                    ((string)record.NameAr)
                    .Trim();

                var nameEn =
                    ((string)record.NameEn)
                    .Trim();

                var descriptionAr =
                    NullIfEmpty(
                        (string?)record.DescriptionAr);

                var descriptionEn =
                    NullIfEmpty(
                        (string?)record.DescriptionEn);

                var displayOrder =
                    int.Parse(
                        (string)record.DisplayOrder,
                        CultureInfo.InvariantCulture);

                var isActive =
                    bool.Parse(
                        (string)record.IsActive);

                if (existingIds.Contains(id)
                    || existingBackends.Contains(backendName))
                {
                    continue;
                }

                if (!skillTypeByBackend.TryGetValue(
                        skillTypeBackendName,
                        out var skillTypeId))
                {
                    context.Errors.Add(
                        new ImportError(
                            FileName,
                            row,
                            $"SkillTypeBackendName '{skillTypeBackendName}' " +
                            "was not found in the database."));

                    continue;
                }

                context.Db.Skill.Add(
                    new Skill
                    {
                        Id = id,
                        SkillTypeId = skillTypeId,
                        BackendName = backendName,
                        NameAr = nameAr,
                        NameEn = nameEn,
                        DescriptionAr = descriptionAr,
                        DescriptionEn = descriptionEn,
                        DisplayOrder = displayOrder,
                        IsActive = isActive,
                        IsDeleted = false,
                        CreatedById = createdById,
                        CreatedDate = now
                    });

                existingIds.Add(id);
                existingBackends.Add(backendName);
            }
            catch (Exception ex)
            {
                context.Errors.Add(
                    new ImportError(
                        FileName,
                        row,
                        ex.GetBaseException().Message));
            }
        }

        return Task.CompletedTask;
    }

    private static string? NullIfEmpty(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var trimmed = value.Trim();

        return trimmed.Equals(
            "NULL",
            StringComparison.OrdinalIgnoreCase)
            ? null
            : trimmed;
    }
}
