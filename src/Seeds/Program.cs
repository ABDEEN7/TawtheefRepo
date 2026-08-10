using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Seeds.ConsoleUi;
using Seeds.Importing;
using Seeds.Models;
using Seeds.Pipeline;
using Seeds.Validation;
using Serilog;
using Tawtheef.Application.Common.Interfaces.Logging;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Infrastructure.Data;
using Tawtheef.Infrastructure.Services.Identity;
using Tawtheef.Infrastructure.Services.Logging;
using Tawtheef.Infrastructure.Services.Security;

namespace Seeds;

public static class Program
{
    public static async Task Main(string[] args)
    {
        Console.Title = "Careers Import Tool";
        ConsoleUi.ConsoleUi.PrintHeader("Careers Seed Import");

        // 1) Connection string selection
        var savedConnections = new List<SavedConnection>
        {
            new("LocalDB", @"Server=(localdb)\MSSQLLocalDB;Database=TawtheefDB;Trusted_Connection=True;")
        };

        var preStageConnectionString =
            Environment.GetEnvironmentVariable("TAWTHEEF_PRESTAGE_CONNECTION_STRING");

        if (!string.IsNullOrWhiteSpace(preStageConnectionString))
            savedConnections.Add(new SavedConnection("PreStage", preStageConnectionString));

        var connectionString = ConsoleUi.ConsoleUi.PromptConnectionString(savedConnections);

        // 2) Import tasks (multi-select)
        var selected = ConsoleUi.ConsoleUi.PromptMultiSelect(
            title: "Select import steps (multi-select)",
            items: ImportCatalog.All,
            render: x => $"{x.Code,-10} {x.Title}",
            allowAllKeyword: true);
        selected = ImportCatalog.ExpandDependencies(selected);

        if (selected.Count == 0)
        {
            ConsoleUi.ConsoleUi.Warn("No steps selected. Exiting.");
            return;
        }

        ConsoleUi.ConsoleUi.PrintSection("Summary");
        Console.WriteLine($"Connection: {ConsoleUi.ConsoleUi.MaskConnection(connectionString)}");
        Console.WriteLine("Steps:");
        foreach (var step in selected) Console.WriteLine($"  - {step.Title} ({step.Code})");
        Console.WriteLine();

        ConsoleUi.ConsoleUi.PrintSection("Preflight validation");
        var preflight = SeedPreflightValidator.Validate(selected);

        foreach ((string file, int count) in preflight.RowCounts)
            Console.WriteLine($"  [DATA] {file,-24} {count,6} rows");

        if (preflight.Errors.Count > 0)
        {
            ConsoleUi.ConsoleUi.Error("Preflight validation failed. No database changes were made.");
            foreach (var e in preflight.Errors)
                Console.WriteLine($"{e.FileName} - Row {e.RowNumber?.ToString() ?? "-"}: {e.Message}");

            ConsoleUi.ConsoleUi.Footer();
            return;
        }

        ConsoleUi.ConsoleUi.Ok("Preflight validation passed.");

        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();

        using var host = Host.CreateDefaultBuilder(args)
            .UseSerilog()
            .ConfigureServices((_, services) =>
            {
                services.AddSingleton(Log.Logger);
                services.AddSingleton<IAppLogger>(_ => new SerilogAppLogger(Log.Logger));
                services.AddScoped<IIdentityFieldProtectionContext, IdentityFieldProtectionContext>();
                services.AddDbContext<TawtheefDbContext>(opt => opt.UseSqlServer(connectionString));
                services.AddScoped<CurrentUserService>();
            })
            .Build();

        using var scope = host.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TawtheefDbContext>();

        var errors = new List<ImportError>();
        var ct = CancellationToken.None;

        ConsoleUi.ConsoleUi.PrintSection("Running");
        var runner = new ImportRunner(db, errors);
        var aborted = false;

        foreach (var step in selected)
        {
            ConsoleUi.ConsoleUi.StepStart(step.Title);
            var errorsBeforeStep = errors.Count;

            try
            {
                if (step.UseTransaction)
                {
                    await using var transaction = await db.Database.BeginTransactionAsync(ct);
                    try
                    {
                        await step.RunAsync(runner, ct);
                        ThrowIfStepReportedErrors(step, errors, errorsBeforeStep);
                        await db.SaveChangesAsync(ct);
                        await transaction.CommitAsync(ct);
                    }
                    catch
                    {
                        await transaction.RollbackAsync(ct);
                        throw;
                    }
                }
                else
                {
                    // APPLICANTS owns its transaction-per-batch strategy.
                    await step.RunAsync(runner, ct);
                    ThrowIfStepReportedErrors(step, errors, errorsBeforeStep);
                    await db.SaveChangesAsync(ct);
                }

                ConsoleUi.ConsoleUi.StepOk(step.Title);
            }
            catch (Exception ex)
            {
                db.ChangeTracker.Clear();

                Log.Error(ex, "Step failed: {Step}", step.Title);

                if (errors.Count == errorsBeforeStep)
                    errors.Add(new ImportError(step.Code, null, ex.GetBaseException().Message));

                ConsoleUi.ConsoleUi.StepFail(step.Title, ex.GetBaseException().Message);
                aborted = true;
                break;
            }
        }

        ConsoleUi.ConsoleUi.PrintSection("Finished");
        if (errors.Any())
        {
            ConsoleUi.ConsoleUi.Error(aborted
                ? "SEED ABORTED - ERRORS SUMMARY"
                : "ERRORS SUMMARY");

            foreach (var e in errors)
                Console.WriteLine($"{e.FileName} - Row {e.RowNumber?.ToString() ?? "-"}: {e.Message}");
        }
        else
        {
            ConsoleUi.ConsoleUi.Ok("Seed completed with no reported errors.");
        }

        ConsoleUi.ConsoleUi.Footer();

    }

    private static void ThrowIfStepReportedErrors(
        ImportStep step,
        IReadOnlyList<ImportError> errors,
        int errorsBeforeStep)
    {
        if (errors.Count <= errorsBeforeStep)
            return;

        var first = errors[errorsBeforeStep];
        var row = first.RowNumber.HasValue ? $" row {first.RowNumber.Value}" : string.Empty;

        throw new InvalidOperationException(
            $"{step.Code} reported {errors.Count - errorsBeforeStep} error(s). " +
            $"First error: {first.FileName}{row}: {first.Message}");
    }
}
