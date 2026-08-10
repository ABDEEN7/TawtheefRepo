using System.Globalization;
using CsvHelper;
using Microsoft.EntityFrameworkCore;
using Seeds.Models;
using Seeds.SeedData;
using Tawtheef.Domain.Entities.Lookups;

namespace Seeds.Importing.Lookups;

internal static class SkillTypeImporter
{
    private const string FileName = "SkillTypeData.csv";

    public static Task ImportAsync(
        SeedImportContext context,
        CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(context);

        var path = SeedDataPath.Get(FileName);

        using var reader = new StreamReader(path);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        var existingIds =
            new HashSet<Guid>(
                context.Db.SkillType
                    .AsNoTracking()
                    .Select(x => x.Id));

        var existingBackends =
            new HashSet<string>(
                context.Db.SkillType
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

                var backendName =
                    ((string)record.BackendName).Trim();

                var nameAr =
                    ((string)record.NameAr).Trim();

                var nameEn =
                    ((string)record.NameEn).Trim();

                var displayOrder =
                    int.Parse(
                        (string)record.DisplayOrder,
                        CultureInfo.InvariantCulture);

                if (existingIds.Contains(id)
                    || existingBackends.Contains(backendName))
                {
                    continue;
                }

                context.Db.SkillType.Add(
                    new SkillType
                    {
                        Id = id,
                        BackendName = backendName,
                        NameAr = nameAr,
                        NameEn = nameEn,
                        DisplayOrder = displayOrder,
                        IsDeleted = false,
                        CreatedDate = DateTime.UtcNow
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
}
