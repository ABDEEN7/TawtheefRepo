using System.Globalization;
using CsvHelper;
using Seeds.Models;
using Seeds.SeedData;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;
using Tawtheef.Domain.Entities.Users;

namespace Seeds.Importing.Lookups;

internal static class JobTitleImporter
{
    private const string FileName = "JobTitlesData.csv";

    public static Task ImportAsync(
        SeedImportContext context,
        CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(context);

        var path = SeedDataPath.Get(FileName);

        using var reader = new StreamReader(path);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        var existingByJobNumber =
            context.Db.JobTitle.ToDictionary(
                x => x.JobNumber,
                StringComparer.OrdinalIgnoreCase);

        var createdById = AdminUserIds.Admin1UserId;
        var now = DateTime.UtcNow;

        while (csv.Read())
        {
            ct.ThrowIfCancellationRequested();

            var row = csv.Context.Parser?.Row;

            try
            {
                dynamic record = csv.GetRecord<dynamic>();

                var jobNumber =
                    ((string?)record.JobNumber)?.Trim();

                var jobNameAr =
                    ((string?)record.JobNameAr)?.Trim();

                var jobNameEn =
                    ((string?)record.JobNameEn)?.Trim();

                if (string.IsNullOrWhiteSpace(jobNumber))
                {
                    context.Errors.Add(
                        new ImportError(
                            FileName,
                            row,
                            "JobNumber is required."));

                    continue;
                }

                if (string.IsNullOrWhiteSpace(jobNameAr))
                {
                    context.Errors.Add(
                        new ImportError(
                            FileName,
                            row,
                            $"JobNameAr is required for JobNumber: {jobNumber}."));

                    continue;
                }

                if (string.IsNullOrWhiteSpace(jobNameEn))
                    jobNameEn = jobNameAr;

                if (existingByJobNumber.TryGetValue(
                        jobNumber,
                        out var existing))
                {
                    existing.JobNameAr =
                        jobNameAr[
                            ..Math.Min(
                                200,
                                jobNameAr.Length)];

                    existing.JobNameEn =
                        jobNameEn[
                            ..Math.Min(
                                200,
                                jobNameEn.Length)];

                    existing.IsActive = true;
                    existing.UpdatedById = createdById;
                    existing.UpdatedDate = now;

                    continue;
                }

                var entity = new JobTitle
                {
                    Id = Guid.NewGuid(),

                    JobNumber =
                        jobNumber[
                            ..Math.Min(
                                100,
                                jobNumber.Length)],

                    JobNameAr =
                        jobNameAr[
                            ..Math.Min(
                                200,
                                jobNameAr.Length)],

                    JobNameEn =
                        jobNameEn[
                            ..Math.Min(
                                200,
                                jobNameEn.Length)],

                    IsActive = true,
                    CreatedById = createdById,
                    CreatedDate = now,
                    IsDeleted = false
                };

                context.Db.JobTitle.Add(entity);

                existingByJobNumber[jobNumber] = entity;
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
