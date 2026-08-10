using System.Globalization;
using CsvHelper;
using Seeds.Models;
using Seeds.SeedData;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Seeds.Importing.Lookups;

internal static class MajorImporter
{
    private const string FileName = "MajorData.csv";

    public static Task ImportAsync(
        SeedImportContext context,
        CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(context);

        var state = context.Majors;
        var path = SeedDataPath.Get(FileName);

        using var reader = new StreamReader(path);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        var rows = new List<(int Row, dynamic Record)>();

        while (csv.Read())
        {
            ct.ThrowIfCancellationRequested();

            rows.Add((
                csv.Context.Parser?.Row ?? 0,
                csv.GetRecord<dynamic>()));
        }

        var remaining = rows.ToList();
        var insertedInThisRun = new HashSet<Guid>();

        var safety = 0;

        while (remaining.Count > 0 && safety++ < 10_000)
        {
            ct.ThrowIfCancellationRequested();

            var ready = remaining
                .Where(item =>
                {
                    string? parentIdText =
                        item.Record.ParentId;

                    if (string.IsNullOrWhiteSpace(parentIdText))
                        return true;

                    var parentId =
                        Guid.Parse(parentIdText);

                    return state.ContainsId(parentId)
                           || insertedInThisRun.Contains(parentId);
                })
                .ToList();

            if (ready.Count == 0)
            {
                foreach (var item in remaining)
                {
                    context.Errors.Add(
                        new ImportError(
                            FileName,
                            item.Row,
                            $"Parent not found for child: " +
                            $"{item.Record.Id} | " +
                            $"Parent: {item.Record.ParentId}"));
                }

                break;
            }

            foreach (var item in ready)
            {
                ct.ThrowIfCancellationRequested();

                try
                {
                    var id =
                        Guid.Parse(
                            (string)item.Record.Id);

                    if (!state.TryRegisterId(
                            id,
                            context.Errors,
                            FileName,
                            item.Row))
                    {
                        remaining.Remove(item);
                        continue;
                    }

                    var backendName =
                        SeedValueNormalizer.ToBackendName(
                            (string)item.Record.BackendName);

                    if (!state.TryRegisterBackend(
                            backendName,
                            context.Errors,
                            FileName,
                            item.Row))
                    {
                        remaining.Remove(item);
                        continue;
                    }

                    Guid? parentId =
                        string.IsNullOrWhiteSpace(
                            item.Record.ParentId)
                            ? null
                            : Guid.Parse(
                                item.Record.ParentId);

                    var entity = new Major
                    {
                        Id = id,
                        BackendName = backendName,
                        NameAr = item.Record.NameAr,
                        NameEn = item.Record.NameEn,
                        DescriptionAr = item.Record.DescriptionAr,
                        DescriptionEn = item.Record.DescriptionEn,
                        DisplayOrder = int.Parse(
                            (string)item.Record.DisplayOrder,
                            CultureInfo.InvariantCulture),
                        ParentId = parentId
                    };

                    context.Db.Major.Add(entity);

                    insertedInThisRun.Add(id);
                }
                catch (Exception ex)
                {
                    context.Errors.Add(
                        new ImportError(
                            FileName,
                            item.Row,
                            ex.GetBaseException().Message));
                }

                remaining.Remove(item);
            }
        }

        return Task.CompletedTask;
    }
}
