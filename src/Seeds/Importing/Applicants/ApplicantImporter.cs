using Tawtheef.Domain.Entities.Users;

namespace Seeds.Importing.Applicants;

internal static class ApplicantImporter
{
    public static async Task ImportAsync(
        SeedImportContext context,
        CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(context);

        var seeder =
            new SmartUserProfileSeeder(
                context.Db);

        await seeder.SeedSmartApplicantsAsync(
            createdById: AdminUserIds.Admin1UserId,
            options: new SmartUserProfileSeeder.SeedOptions
            {
                Count = 10_000,
                BatchSize = 1_000,
                CompletionRate = 0.85,

                ProfileAttachmentMode =
                    SmartUserProfileSeeder
                        .AttachmentMode
                        .Pooled,

                CertificateAttachmentMode =
                    SmartUserProfileSeeder
                        .AttachmentMode
                        .Pooled,

                UseTransactionPerBatch = true
            },
            ct: ct);
    }
}
