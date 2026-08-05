using System.Globalization;
using CsvHelper;
using Seeds.Models;
using Seeds.SeedData;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Seeds.Importing.Lookups;

internal static class UniversityImporter
{
    private const string FileName = "UniversityData.csv";

    public static Task ImportAsync(
        SeedImportContext context,
        CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(context);

        var state = context.Universities;
        var cities = context.Cities;
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
                var cityId = Guid.Parse((string)record.CityId);

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

                if (!cities.ContainsId(cityId))
                {
                    context.Errors.Add(
                        new ImportError(
                            FileName,
                            row,
                            $"Invalid CityId (no FK found): {cityId}"));

                    continue;
                }

                context.Db.University.Add(new University
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
                    CityId = cityId,
                    WebSite = record.Website,
                    Phone = record.Phone,
                    Email = record.Email,
                    Code = record.Code,
                    OriginalName = record.OriginalName
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
