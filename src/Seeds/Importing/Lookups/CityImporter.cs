using System.Globalization;
using CsvHelper;
using Seeds.Models;
using Seeds.SeedData;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Seeds.Importing.Lookups;

internal static class CityImporter
{
    private const string FileName = "CityData.csv";

    public static Task ImportAsync(
        SeedImportContext context,
        CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(context);

        var state = context.Cities;
        var countries = context.Countries;
        var path = SeedDataPath.Get(FileName);

        using var reader = new StreamReader(path);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        while (csv.Read())
        {
            ct.ThrowIfCancellationRequested();

            var row = csv.Context.Parser?.Row;

            try
            {
                dynamic record = csv.GetRecord<dynamic>();

                var id = Guid.Parse((string)record.Id);
                var countryId = Guid.Parse((string)record.CountryId);

                var backendName =
                    SeedValueNormalizer.ToBackendName(
                        (string)record.NameEn);

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

                if (!countries.ContainsId(countryId))
                {
                    context.Errors.Add(
                        new ImportError(
                            FileName,
                            row,
                            $"Invalid CountryId (no FK found): {countryId}"));

                    continue;
                }

                context.Db.City.Add(new City
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
                    CountryId = countryId,
                    Code = record.BackendName
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
