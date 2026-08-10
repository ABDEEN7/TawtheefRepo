using System.Globalization;
using CsvHelper;
using Seeds.Models;
using Seeds.SeedData;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Seeds.Importing.Lookups;

internal static class CountryImporter
{
    private const string FileName = "CountryData.csv";

    public static Task ImportAsync(
        SeedImportContext context,
        CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(context);

        ct.ThrowIfCancellationRequested();

        var state = context.Countries;
        var path = SeedDataPath.Get(FileName);

        using var reader = new StreamReader(path);
        using var csv = new CsvReader(
            reader,
            CultureInfo.InvariantCulture);

        while (csv.Read())
        {
            ct.ThrowIfCancellationRequested();

            var row = csv.Context.Parser?.Row;

            try
            {
                dynamic record = csv.GetRecord<dynamic>();

                var id = Guid.Parse(
                    (string)record.Id);

                var backendNameRaw =
                    ((string)record.BackendName).Trim();

                var backendName =
                    backendNameRaw[
                        ..Math.Min(50, backendNameRaw.Length)];

                if (!state.TryRegisterId(
                        id,
                        context.Errors,
                        FileName,
                        row))
                {
                    continue;
                }

                if (!state.TryRegisterBackend(
                        backendName,
                        context.Errors,
                        FileName,
                        row))
                {
                    continue;
                }

                context.Db.Country.Add(new Country
                {
                    Id = id,
                    BackendName = backendName,
                    NameAr = record.NameAr,
                    NameEn = record.NameEn,
                    DescriptionAr = record.DescriptionAr,
                    DescriptionEn = record.DescriptionEn,
                    DisplayOrder = int.Parse(
                        (string)record.DisplayOrder,
                        CultureInfo.InvariantCulture),
                    Code = int.Parse(
                        (string)record.Code,
                        CultureInfo.InvariantCulture),
                    ISOCode = record.ISOCode ?? string.Empty,
                    CodeAlpha = record.CodeAlpha ?? string.Empty
                });
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
