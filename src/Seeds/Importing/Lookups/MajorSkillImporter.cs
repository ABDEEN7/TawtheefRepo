using System.Globalization;
using CsvHelper;
using Microsoft.EntityFrameworkCore;
using Seeds.Models;
using Seeds.SeedData;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;
using Tawtheef.Domain.Entities.Users;

namespace Seeds.Importing.Lookups;

internal static class MajorSkillImporter
{
    private const string FileName = "MajorSkillData.csv";

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

        var validMajorIds =
            new HashSet<Guid>(
                context.Db.Major
                    .AsNoTracking()
                    .Where(x => !x.IsDeleted)
                    .Select(x => x.Id));

        var skillByBackend =
            context.Db.Skill
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .ToDictionary(
                    x => x.BackendName,
                    x => x.Id,
                    StringComparer.OrdinalIgnoreCase);

        var existingIds =
            new HashSet<Guid>(
                context.Db.Set<MajorSkill>()
                    .AsNoTracking()
                    .Select(x => x.Id));

        var existingLinks =
            new HashSet<(Guid MajorId, Guid SkillId)>(
                context.Db.Set<MajorSkill>()
                    .AsNoTracking()
                    .Where(x => !x.IsDeleted)
                    .Select(x =>
                        new ValueTuple<Guid, Guid>(
                            x.MajorId,
                            x.SkillId)));

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

                var majorId =
                    Guid.Parse(
                        (string)record.MajorId);

                var skillBackendName =
                    ((string)record.SkillBackendName)
                    .Trim();

                if (!validMajorIds.Contains(majorId))
                {
                    context.Errors.Add(
                        new ImportError(
                            FileName,
                            row,
                            $"MajorId '{majorId}' was not found in the database."));

                    continue;
                }

                if (!skillByBackend.TryGetValue(
                        skillBackendName,
                        out var skillId))
                {
                    context.Errors.Add(
                        new ImportError(
                            FileName,
                            row,
                            $"SkillBackendName '{skillBackendName}' " +
                            "was not found in the database."));

                    continue;
                }

                if (existingIds.Contains(id)
                    || existingLinks.Contains(
                        (majorId, skillId)))
                {
                    continue;
                }

                context.Db.Set<MajorSkill>()
                    .Add(new MajorSkill
                    {
                        Id = id,
                        MajorId = majorId,
                        SkillId = skillId,
                        IsActive = true,
                        IsDeleted = false,
                        CreatedById = createdById,
                        CreatedDate = now
                    });

                existingIds.Add(id);

                existingLinks.Add(
                    (majorId, skillId));
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
