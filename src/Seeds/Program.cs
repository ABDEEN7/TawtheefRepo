using System.Globalization;
using CsvHelper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Tawtheef.Application.Common.Interfaces.Logging;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Infrastructure.Data;
using Tawtheef.Infrastructure.Services.Identity;
using Tawtheef.Infrastructure.Services.Logging;
using Tawtheef.Infrastructure.Services.Security;

namespace Seeds;

public static class Program
{
    public static async Task Main(string[] args)
    {
        Console.Title = "Tawtheef Import Tool";
        ConsoleUi.PrintHeader("Tawtheef Seed Import");

        // 1) Connection string selection
        var connectionString = ConsoleUi.PromptConnectionString([
            new SavedConnection("LocalDB", @"Server=(localdb)\MSSQLLocalDB;Database=TawtheefDB;Trusted_Connection=True;"),
            new SavedConnection("PreStage", "Server=DCDCSQL2DNET01;Database=Tawthef;Trust Server Certificate=true;User id=tawthef_user;Password=Abc@1234;")
        ]);

        // 2) Import tasks (multi-select)
        var selected = ConsoleUi.PromptMultiSelect(
            title: "Select import steps (multi-select)",
            items: ImportCatalog.All,
            render: x => $"{x.Code,-10} {x.Title}",
            allowAllKeyword: true);

        if (selected.Count == 0)
        {
            ConsoleUi.Warn("No steps selected. Exiting.");
            return;
        }

        ConsoleUi.PrintSection("Summary");
        Console.WriteLine($"Connection: {ConsoleUi.MaskConnection(connectionString)}");
        Console.WriteLine("Steps:");
        foreach (var step in selected) Console.WriteLine($"  - {step.Title} ({step.Code})");
        Console.WriteLine();

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

        ConsoleUi.PrintSection("Running");
        var runner = new ImportRunner(db, errors);

        foreach (var step in selected)
        {
            ConsoleUi.StepStart(step.Title);

            try
            {
                await step.RunAsync(runner, ct);
                ConsoleUi.StepOk(step.Title);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Step failed: {Step}", step.Title);
                errors.Add(new ImportError(step.Code, null, ex.GetBaseException().Message));
                ConsoleUi.StepFail(step.Title, ex.GetBaseException().Message);
            }
        }

        // Save once at the end
        ConsoleUi.PrintSection("Saving changes");
        try
        {
            await db.SaveChangesAsync(ct);
            ConsoleUi.Ok("SaveChanges succeeded.");
        }
        catch (DbUpdateException ex)
        {
            Log.Error(ex, "SaveChanges failed");
            errors.Add(new ImportError("DB", 0, ex.GetBaseException().Message));
            ConsoleUi.Error("SaveChanges failed.");
        }

        ConsoleUi.PrintSection("Finished");
        if (errors.Any())
        {
            ConsoleUi.Error("ERRORS SUMMARY");
            foreach (var e in errors)
                Console.WriteLine($"{e.FileName} - Row {e.RowNumber?.ToString() ?? "-"}: {e.Message}");
        }
        else
        {
            ConsoleUi.Ok("No errors.");
        }

        ConsoleUi.Footer();
    }
}

#region Console UI

internal static class ConsoleUi
{
    public static void PrintHeader(string title)
    {
        Console.WriteLine(new string('=', 72));
        Console.WriteLine($"  {title}");
        Console.WriteLine(new string('=', 72));
        Console.WriteLine();
    }

    public static void Footer()
    {
        Console.WriteLine();
        Console.WriteLine(new string('-', 72));
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey(intercept: true);
    }

    public static void PrintSection(string title)
    {
        Console.WriteLine();
        Console.WriteLine($"--- {title} ---");
    }

    public static void Ok(string message) => WriteColored(message, ConsoleColor.Green);
    public static void Warn(string message) => WriteColored(message, ConsoleColor.Yellow);
    public static void Error(string message) => WriteColored(message, ConsoleColor.Red);

    public static void StepStart(string title) => Console.Write($"[....] {title} ");

    public static void StepOk(string title)
    {
        Console.WriteLine();
        WriteColored($"[ OK ] {title}", ConsoleColor.Green);
    }

    public static void StepFail(string title, string reason)
    {
        Console.WriteLine();
        WriteColored($"[FAIL] {title} :: {reason}", ConsoleColor.Red);
    }

    public static string PromptConnectionString(IReadOnlyList<SavedConnection> presets)
    {
        PrintSection("Connection");
        Console.WriteLine("Choose a connection string:");
        for (var i = 0; i < presets.Count; i++)
            Console.WriteLine($"  {i + 1}) {presets[i].Name}  ({MaskConnection(presets[i].ConnectionString)})");
        Console.WriteLine($"  {presets.Count + 1}) Enter a new connection string");
        Console.WriteLine();

        while (true)
        {
            var choice = PromptInt($"Select [1..{presets.Count + 1}]: ", 1, presets.Count + 1);

            if (choice <= presets.Count)
                return presets[choice - 1].ConnectionString;

            var entered = PromptText("Paste connection string: ");
            if (IsLikelyConnectionString(entered))
                return entered;

            Warn("That does not look like a valid SQL Server connection string. Try again.");
        }
    }

    public static string MaskConnection(string cs)
    {
        if (string.IsNullOrWhiteSpace(cs)) return cs;

        var parts = cs.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
        for (int i = 0; i < parts.Count; i++)
        {
            if (parts[i].StartsWith("Password=", StringComparison.OrdinalIgnoreCase) ||
                parts[i].StartsWith("Pwd=", StringComparison.OrdinalIgnoreCase))
            {
                var key = parts[i].Split('=')[0];
                parts[i] = $"{key}=***";
            }
        }
        return string.Join("; ", parts) + ";";
    }

    public static List<T> PromptMultiSelect<T>(
        string title,
        IReadOnlyList<T> items,
        Func<T, string> render,
        bool allowAllKeyword)
    {
        PrintSection(title);
        for (var i = 0; i < items.Count; i++)
            Console.WriteLine($"  {i + 1,2}) {render(items[i])}");

        if (allowAllKeyword)
        {
            Console.WriteLine();
            Console.WriteLine("Type: 1,3,5  or 2-6  or all");
        }

        Console.WriteLine();

        while (true)
        {
            var raw = PromptText("Select: ").Trim();

            if (allowAllKeyword && raw.Equals("all", StringComparison.OrdinalIgnoreCase))
                return items.ToList();

            var indices = ParseMultiSelection(raw, items.Count);
            if (indices.Count == 0)
            {
                Warn("Invalid selection. Examples: 1,3,5 or 2-6 or all");
                continue;
            }

            return indices.Select(i => items[i - 1]).ToList();
        }
    }

    private static List<int> ParseMultiSelection(string input, int max)
    {
        var result = new HashSet<int>();
        if (string.IsNullOrWhiteSpace(input)) return [];

        var tokens = input.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        foreach (var token in tokens)
        {
            if (token.Contains('-', StringComparison.Ordinal))
            {
                var range = token.Split('-', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                if (range.Length != 2) return [];
                if (!int.TryParse(range[0], out var a)) return [];
                if (!int.TryParse(range[1], out var b)) return [];
                if (a < 1 || b < 1 || a > max || b > max) return [];
                if (a > b) (a, b) = (b, a);
                for (var i = a; i <= b; i++) result.Add(i);
            }
            else
            {
                if (!int.TryParse(token, out var n)) return [];
                if (n < 1 || n > max) return [];
                result.Add(n);
            }
        }
        return result.OrderBy(x => x).ToList();
    }

    private static int PromptInt(string message, int min, int max)
    {
        while (true)
        {
            Console.Write(message);
            var raw = Console.ReadLine();
            if (int.TryParse(raw, out var n) && n >= min && n <= max)
                return n;

            Warn($"Enter a number between {min} and {max}.");
        }
    }

    private static string PromptText(string message)
    {
        Console.Write(message);
        return Console.ReadLine() ?? string.Empty;
    }

    private static bool IsLikelyConnectionString(string cs)
        => cs.Contains("Server=", StringComparison.OrdinalIgnoreCase) ||
           cs.Contains("Data Source=", StringComparison.OrdinalIgnoreCase);

    private static void WriteColored(string text, ConsoleColor color)
    {
        var old = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.WriteLine(text);
        Console.ForegroundColor = old;
    }
}

internal sealed record SavedConnection(string Name, string ConnectionString);

#endregion

#region Import Catalog + Runner

internal sealed class ImportStep
{
    public required string Code { get; init; }
    public required string Title { get; init; }
    public required Func<ImportRunner, CancellationToken, Task> RunAsync { get; init; }
}

internal static class ImportCatalog
{
    public static readonly List<ImportStep> All =
    [
        new ImportStep { Code = "COUNTRY", Title = "Import Countries (CSV)", RunAsync = (r, ct) => r.ImportCountriesAsync(ct) },
        new ImportStep { Code = "CITY",    Title = "Import Cities (CSV)",    RunAsync = (r, ct) => r.ImportCitiesAsync(ct) },
        new ImportStep { Code = "UNIV",    Title = "Import Universities (CSV)", RunAsync = (r, ct) => r.ImportUniversitiesAsync(ct) },
        new ImportStep { Code = "MAJOR",   Title = "Import Majors (CSV)",    RunAsync = (r, ct) => r.ImportMajorsAsync(ct) },
        new ImportStep { Code = "JTITLE",  Title = "Import Job Titles (CSV)", RunAsync = (r, ct) => r.ImportJobTitlesAsync(ct) },
        new ImportStep { Code = "OFFICE",  Title = "Insert Offices (Code)",  RunAsync = (r, ct) => r.ImportOfficesAsync(ct) },
        new ImportStep { Code = "SKTYPE",  Title = "Insert SkillTypes (Code)", RunAsync = (r, ct) => r.ImportSkillTypesAsync(ct) },
        new ImportStep { Code = "SKILL",   Title = "Insert Skills (Code)",   RunAsync = (r, ct) => r.ImportSkillsAsync(ct) },
        new ImportStep { Code = "MSLINK",  Title = "Insert Major-Skill Links (Code)", RunAsync = (r, ct) => r.ImportMajorSkillsAsync(ct) },
        new ImportStep
        {
            Code = "APPLICANTS",
            Title = "Seed 10k Applicant Users + Profiles (Smart)",
            RunAsync = (r, ct) => r.SeedApplicantsAsync(ct)
        },
    ];
}

internal sealed class ImportRunner
{
    private readonly TawtheefDbContext _db;
    private readonly List<ImportError> _errors;

    // Cached preloads (only loaded if/when needed)
    private HashSet<Guid>? _countryIds;
    private HashSet<string>? _countryBackends;

    private HashSet<Guid>? _cityIds;
    private HashSet<string>? _cityBackends;

    private HashSet<Guid>? _univIds;
    private HashSet<string>? _univBackends;

    private HashSet<Guid>? _majorIds;
    private HashSet<string>? _majorBackends;

    public ImportRunner(TawtheefDbContext db, List<ImportError> errors)
    {
        _db = db;
        _errors = errors;
    }

    public Task ImportCountriesAsync(CancellationToken ct)
    {
        EnsurePreloadCountries();
        ImportCountries(_db, _errors, _countryIds!, _countryBackends!);
        return Task.CompletedTask;
    }

    public Task ImportCitiesAsync(CancellationToken ct)
    {
        EnsurePreloadCountries();
        EnsurePreloadCities();
        ImportCities(_db, _errors, _cityIds!, _cityBackends!, _countryIds!);
        return Task.CompletedTask;
    }

    public Task ImportUniversitiesAsync(CancellationToken ct)
    {
        EnsurePreloadCities();
        EnsurePreloadUniversities();
        ImportUniversities(_db, _errors, _univIds!, _univBackends!, _cityIds!);
        return Task.CompletedTask;
    }

    public Task ImportMajorsAsync(CancellationToken ct)
    {
        EnsurePreloadMajors();
        ImportMajors(_db, _errors, _majorIds!, _majorBackends!);
        return Task.CompletedTask;
    }

    public Task ImportJobTitlesAsync(CancellationToken ct)
    {
        ImportJobTitles(_db, _errors);
        return Task.CompletedTask;
    }

    public Task ImportOfficesAsync(CancellationToken ct)
    {
        ImportOffices(_db, _errors);
        return Task.CompletedTask;
    }

    public Task ImportSkillTypesAsync(CancellationToken ct)
    {
        ImportSkillTypes(_db, _errors);
        return Task.CompletedTask;
    }

    public Task ImportSkillsAsync(CancellationToken ct)
    {
        ImportSkills(_db, _errors);
        return Task.CompletedTask;
    }

    public Task ImportMajorSkillsAsync(CancellationToken ct)
    {
        ImportMajorSkills(_db, _errors);
        return Task.CompletedTask;
    }
    public async Task SeedApplicantsAsync(CancellationToken ct)
    {
        // Important: ensure required lookups are present before seeding
        // (Countries, Universities, Majors, Offices, SkillTypes/Skills if needed)

        var adminUserId = AdminUserIds.Admin1UserId;

        var seeder = new SmartUserProfileSeeder(_db);

        await seeder.SeedSmartApplicantsAsync(
            createdById: adminUserId,
            options: new SmartUserProfileSeeder.SeedOptions
            {
                Count = 10_000,
                BatchSize = 1_000,
                CompletionRate = 0.85,
                ProfileAttachmentMode = SmartUserProfileSeeder.AttachmentMode.Pooled,
                CertificateAttachmentMode = SmartUserProfileSeeder.AttachmentMode.Pooled,
                UseTransactionPerBatch = true
            },
            ct: ct
        );
    }
    private void EnsurePreloadCountries()
    {
        if (_countryIds is not null) return;
        _countryIds = new HashSet<Guid>(_db.Country.AsNoTracking().Select(x => x.Id));
        _countryBackends = new HashSet<string>(_db.Country.AsNoTracking().Select(x => x.BackendName),
            StringComparer.OrdinalIgnoreCase);
    }

    private void EnsurePreloadCities()
    {
        if (_cityIds is not null) return;
        _cityIds = new HashSet<Guid>(_db.City.AsNoTracking().Select(x => x.Id));
        _cityBackends = new HashSet<string>(_db.City.AsNoTracking().Select(x => x.BackendName),
            StringComparer.OrdinalIgnoreCase);
    }

    private void EnsurePreloadUniversities()
    {
        if (_univIds is not null) return;
        _univIds = new HashSet<Guid>(_db.University.AsNoTracking().Select(x => x.Id));
        _univBackends = new HashSet<string>(_db.University.AsNoTracking().Select(x => x.BackendName),
            StringComparer.OrdinalIgnoreCase);
    }

    private void EnsurePreloadMajors()
    {
        if (_majorIds is not null) return;
        _majorIds = new HashSet<Guid>(_db.Major.AsNoTracking().Select(x => x.Id));
        _majorBackends = new HashSet<string>(_db.Major.AsNoTracking().Select(x => x.BackendName),
            StringComparer.OrdinalIgnoreCase);
    }

    // ================= Helpers =================

    private static string GetDataPath(string fileName)
    {
        var basePath = AppContext.BaseDirectory;
        return Path.Combine(basePath, "SeedData", fileName);
    }

    private static string ToBackendName(string? nameEn)
    {
        if (string.IsNullOrWhiteSpace(nameEn))
            return string.Empty;

        var cleaned = new string(nameEn
            .Where(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c))
            .ToArray());

        var words = cleaned
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(w => char.ToUpper(w[0]) + w[1..].ToLower())
            .ToArray();

        var joined = string.Join("", words);
        return joined.Length > 50 ? joined[..50] : joined;
    }

    private static bool GuardId(HashSet<Guid> existingIds, List<ImportError> errors, string file, int? row, Guid id)
    {
        if (existingIds.Add(id)) return true;
        errors.Add(new ImportError(file, row, $"Duplicate Id: {id}"));
        return false;
    }

    private static bool GuardBackend(HashSet<string> existingBackends, List<ImportError> errors, string file, int? row, string backend)
    {
        if (string.IsNullOrWhiteSpace(backend)) return true;

        if (existingBackends.Add(backend)) return true;

        errors.Add(new ImportError(file, row, $"Duplicate BackendName: {backend}"));
        return false;
    }

    // ================= Import Countries =================

    private static void ImportCountries(
        TawtheefDbContext db,
        List<ImportError> errors,
        HashSet<Guid> existingIds,
        HashSet<string> backendNames)
    {
        var path = GetDataPath("CountryData.csv");
        using var reader = new StreamReader(path);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        while (csv.Read())
        {
            var row = csv.Context.Parser?.Row;
            try
            {
                dynamic r = csv.GetRecord<dynamic>();

                var id = Guid.Parse((string)r.Id);
                var backendNameRaw = (string)r.BackendName;
                var backendName = backendNameRaw[..Math.Min(50, backendNameRaw.Length)];

                if (!GuardId(existingIds, errors, "CountryData.csv", row, id))
                    continue;

                if (!GuardBackend(backendNames, errors, "CountryData.csv", row, backendName))
                    continue;

                db.Country.Add(new Country
                {
                    Id = id,
                    BackendName = backendName,
                    NameAr = r.NameAr,
                    NameEn = r.NameEn,
                    DescriptionAr = r.DescriptionAr,
                    DescriptionEn = r.DescriptionEn,
                    DisplayOrder = int.Parse((string)r.DisplayOrder),
                    Code = int.Parse((string)r.Code),
                    ISOCode = r.ISOCode ?? "",
                    CodeAlpha = r.CodeAlpha ?? ""
                });
            }
            catch (Exception ex)
            {
                errors.Add(new ImportError("CountryData.csv", row, ex.GetBaseException().Message));
            }
        }
    }

    // ================= Import Cities =================

    private static void ImportCities(
        TawtheefDbContext db,
        List<ImportError> errors,
        HashSet<Guid> existingIds,
        HashSet<string> backendNames,
        HashSet<Guid> validCountryIds)
    {
        var path = GetDataPath("CityData.csv");
        using var reader = new StreamReader(path);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        while (csv.Read())
        {
            var row = csv.Context.Parser?.Row;
            try
            {
                dynamic r = csv.GetRecord<dynamic>();

                var id = Guid.Parse((string)r.Id);
                var countryId = Guid.Parse((string)r.CountryId);
                var backendName = ToBackendName((string)r.NameEn);

                if (!GuardId(existingIds, errors, "CityData.csv", row, id))
                    continue;

                if (!GuardBackend(backendNames, errors, "CityData.csv", row, backendName))
                    continue;

                if (!validCountryIds.Contains(countryId))
                {
                    errors.Add(new ImportError("CityData.csv", row, $"Invalid CountryId (no FK found): {countryId}"));
                    continue;
                }

                db.City.Add(new City
                {
                    Id = id,
                    BackendName = backendName,
                    NameAr = r.NameAr,
                    NameEn = r.NameEn,
                    DescriptionAr = r.DescriptionAr,
                    DescriptionEn = r.DescriptionEn,
                    DisplayOrder = int.Parse((string)r.DisplayOrder),
                    CountryId = countryId,
                    Code = r.BackendName
                });
            }
            catch (Exception ex)
            {
                errors.Add(new ImportError("CityData.csv", row, ex.GetBaseException().Message));
            }
        }
    }

    // ================= Import Universities =================

    private static void ImportUniversities(
        TawtheefDbContext db,
        List<ImportError> errors,
        HashSet<Guid> existingIds,
        HashSet<string> backendNames,
        HashSet<Guid> validCityIds)
    {
        var path = GetDataPath("UniversityData.csv");
        using var reader = new StreamReader(path);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        while (csv.Read())
        {
            var row = csv.Context.Parser?.Row;
            try
            {
                dynamic r = csv.GetRecord<dynamic>();

                var id = Guid.Parse((string)r.Id);
                var cityId = Guid.Parse((string)r.CityId);
                var backendName = ToBackendName((string)r.NameEn);

                if (!GuardId(existingIds, errors, "UniversityData.csv", row, id))
                    continue;

                if (!GuardBackend(backendNames, errors, "UniversityData.csv", row, backendName))
                    continue;

                if (!validCityIds.Contains(cityId))
                {
                    errors.Add(new ImportError("UniversityData.csv", row, $"Invalid CityId (no FK found): {cityId}"));
                    continue;
                }

                db.University.Add(new University
                {
                    Id = id,
                    BackendName = backendName,
                    NameAr = r.NameAr,
                    NameEn = r.NameEn,
                    DescriptionAr = r.DescriptionAr,
                    DescriptionEn = r.DescriptionEn,
                    DisplayOrder = int.Parse((string)r.DisplayOrder),
                    CityId = cityId,
                    WebSite = r.Website,
                    Phone = r.Phone,
                    Email = r.Email,
                    Code = r.Code,
                    OriginalName = r.OriginalName
                });
            }
            catch (Exception ex)
            {
                errors.Add(new ImportError("UniversityData.csv", row, ex.GetBaseException().Message));
            }
        }
    }

    // ================= Import Majors (FIXED: avoid duplicate PK Id) =================

    private static void ImportMajors(
        TawtheefDbContext db,
        List<ImportError> errors,
        HashSet<Guid> existingIds,
        HashSet<string> existingBackends)
    {
        var path = GetDataPath("MajorData.csv");
        using var reader = new StreamReader(path);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        var rows = new List<(int Row, dynamic Record)>();
        while (csv.Read())
        {
            var rowNumber = csv.Context.Parser?.Row ?? 0;
            rows.Add((rowNumber, csv.GetRecord<dynamic>()));
        }

        var remaining = rows.ToList();
        var insertedInThisRun = new HashSet<Guid>();

        var safety = 0;
        while (remaining.Any() && safety++ < 10000)
        {
            var ready = remaining
                .Where(x =>
                {
                    string parentIdStr = x.Record.ParentId;
                    if (string.IsNullOrWhiteSpace(parentIdStr))
                        return true;

                    var parentId = Guid.Parse(parentIdStr);

                    // Parent must exist either:
                    // - already in DB (existingIds has it), or
                    // - inserted earlier in this run
                    return existingIds.Contains(parentId) || insertedInThisRun.Contains(parentId);
                })
                .ToList();

            if (!ready.Any())
            {
                foreach (var x in remaining)
                {
                    errors.Add(new ImportError("MajorData.csv", x.Row,
                        $"Parent not found for child: {x.Record.Id} | Parent: {x.Record.ParentId}"));
                }

                break;
            }

            foreach (var x in ready)
            {
                try
                {
                    var id = Guid.Parse((string)x.Record.Id);

                    if (!GuardId(existingIds, errors, "MajorData.csv", x.Row, id))
                    {
                        remaining.Remove(x);
                        continue;
                    }

                    var backend = ToBackendName((string)x.Record.BackendName);
                    if (!GuardBackend(existingBackends, errors, "MajorData.csv", x.Row, backend))
                    {
                        remaining.Remove(x);
                        continue;
                    }

                    Guid? parent = string.IsNullOrWhiteSpace(x.Record.ParentId)
                        ? null
                        : Guid.Parse(x.Record.ParentId);

                    var entity = new Major
                    {
                        Id = id,
                        BackendName = backend,
                        NameAr = x.Record.NameAr,
                        NameEn = x.Record.NameEn,
                        DescriptionAr = x.Record.DescriptionAr,
                        DescriptionEn = x.Record.DescriptionEn,
                        DisplayOrder = int.Parse((string)x.Record.DisplayOrder),
                        ParentId = parent
                    };

                    db.Major.Add(entity);
                    insertedInThisRun.Add(entity.Id);
                }
                catch (Exception ex)
                {
                    errors.Add(new ImportError("MajorData.csv", x.Row, ex.GetBaseException().Message));
                }

                remaining.Remove(x);
            }
        }
    }

    // ================= Import Offices =================

    private static void ImportJobTitles(TawtheefDbContext db, List<ImportError> errors)
    {
        var path = GetDataPath("JobTitlesData.csv");
        using var reader = new StreamReader(path);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        var existingByJobNumber = db.JobTitle
            .ToDictionary(x => x.JobNumber, StringComparer.OrdinalIgnoreCase);

        var createdById = AdminUserIds.Admin1UserId;
        var now = DateTime.UtcNow;

        while (csv.Read())
        {
            var row = csv.Context.Parser?.Row;

            try
            {
                dynamic r = csv.GetRecord<dynamic>();

                var jobNumber = ((string?)r.JobNumber)?.Trim();
                var jobNameAr = ((string?)r.JobNameAr)?.Trim();
                var jobNameEn = ((string?)r.JobNameEn)?.Trim();

                if (string.IsNullOrWhiteSpace(jobNumber))
                {
                    errors.Add(new ImportError("JobTitlesData.csv", row, "JobNumber is required."));
                    continue;
                }

                if (string.IsNullOrWhiteSpace(jobNameAr))
                {
                    errors.Add(new ImportError("JobTitlesData.csv", row, $"JobNameAr is required for JobNumber: {jobNumber}."));
                    continue;
                }

                if (string.IsNullOrWhiteSpace(jobNameEn))
                {
                    errors.Add(new ImportError("JobTitlesData.csv", row, $"JobNameEn is required for JobNumber: {jobNumber}."));
                    continue;
                }

                if (existingByJobNumber.TryGetValue(jobNumber, out var existing))
                {
                    existing.JobNameAr = jobNameAr[..Math.Min(200, jobNameAr.Length)];
                    existing.JobNameEn = jobNameEn[..Math.Min(200, jobNameEn.Length)];
                    existing.IsActive = true;
                    existing.UpdatedById = createdById;
                    existing.UpdatedDate = now;
                    continue;
                }

                var entity = new JobTitle
                {
                    Id = Guid.NewGuid(),
                    JobNumber = jobNumber[..Math.Min(100, jobNumber.Length)],
                    JobNameAr = jobNameAr[..Math.Min(200, jobNameAr.Length)],
                    JobNameEn = jobNameEn[..Math.Min(200, jobNameEn.Length)],
                    IsActive = true,
                    CreatedById = createdById,
                    CreatedDate = now,
                    IsDeleted = false
                };

                db.JobTitle.Add(entity);
                existingByJobNumber[jobNumber] = entity;
            }
            catch (Exception ex)
            {
                errors.Add(new ImportError("JobTitlesData.csv", row, ex.GetBaseException().Message));
            }
        }
    }

    // ================= Import Offices =================

    private static void ImportOffices(TawtheefDbContext db, List<ImportError> errors)
    {
        try
        {
            var jordanId = Guid.Parse("b7f89fce-4f81-464a-9e99-7fc7c8bd1d54");
            var syriaId = Guid.Parse("21d0a391-7e7f-4a3e-bd27-9c26345c7e09");
            var ukId = Guid.Parse("cba347b0-123e-4eb1-91be-ca3a2725bbeb");

            var offices = new (Guid CountryId, string Code, string BackendName, string Ar, string En, int Order)[]
            {
                (jordanId, "JO-AMM", "AMMAN_OFFICE", "مكتب عمّان", "Amman Office", 1),
                (jordanId, "JO-IRB", "IRBID_OFFICE", "مكتب إربد", "Irbid Office", 2),
                (jordanId, "JO-ZAR", "ZARQA_OFFICE", "مكتب الزرقاء", "Zarqa Office", 3),
                (syriaId, "SY-DAM", "DAMASCUS_OFFICE", "مكتب دمشق", "Damascus Office", 4),
                (syriaId, "SY-ALA", "ALEPPO_OFFICE", "مكتب حلب", "Aleppo Office", 5),
                (syriaId, "SY-HOM", "HOMS_OFFICE", "مكتب حمص", "Homs Office", 6),
                (ukId, "UK-LON", "LONDON_OFFICE", "مكتب لندن", "London Office", 7),
                (ukId, "UK-MAN", "MANCHESTER_OFFICE", "مكتب مانشستر", "Manchester Office", 8),
                (ukId, "UK-BIR", "BIRMINGHAM_OFFICE", "مكتب برمنغهام", "Birmingham Office", 9),
                (ukId, "UK-LIV", "LIVERPOOL_OFFICE", "مكتب ليفربول", "Liverpool Office", 10),
            };

            var existing = new HashSet<string>(
                db.Office.AsNoTracking().Select(x => x.BackendName),
                StringComparer.OrdinalIgnoreCase);

            foreach (var o in offices)
            {
                if (!existing.Add(o.BackendName))
                    continue;

                db.Office.Add(new Office
                {
                    Id = Guid.NewGuid(),
                    CreatedDate = DateTime.UtcNow,
                    IsDeleted = false,
                    OfficeAdminId = AdminUserIds.Admin1UserId,
                    CountryId = o.CountryId,
                    Code = o.Code,
                    BackendName = o.BackendName,
                    NameAr = o.Ar,
                    NameEn = o.En,
                    DisplayOrder = o.Order
                });
            }
        }
        catch (Exception ex)
        {
            errors.Add(new ImportError("InsertOffices", null, ex.GetBaseException().Message));
        }
    }

    // ================= Import SkillTypes =================

    private static void ImportSkillTypes(TawtheefDbContext db, List<ImportError> errors)
    {
        try
        {
            var skills = new (string Backend, string Ar, string En, int Order)[]
            {
                ("TECHNICAL", "مهارات تقنية", "Technical Skills", 1), 
                ("SOFT", "مهارات شخصية", "Soft Skills", 2),
                ("LANGUAGE", "مهارات لغوية", "Language Skills", 3),
                ("MANAGEMENT", "مهارات إدارية", "Management Skills", 4),
                ("LEADERSHIP", "مهارات قيادية", "Leadership Skills", 5),
                ("COMPUTER", "مهارات الحاسوب", "Computer Skills", 6),
                ("COMMUNICATION", "مهارات التواصل", "Communication Skills", 7),
                ("CREATIVE", "مهارات إبداعية", "Creative Skills", 8),
                ("ANALYTICAL", "مهارات تحليلية", "Analytical Skills", 9),
                ("OTHER", "مهارات أخرى", "Other Skills", 10)
            };

            var existing = new HashSet<string>(
                db.SkillType.AsNoTracking().Select(x => x.BackendName),
                StringComparer.OrdinalIgnoreCase);

            foreach (var s in skills)
            {
                if (!existing.Add(s.Backend))
                    continue;

                db.SkillType.Add(new SkillType
                {
                    Id = Guid.NewGuid(),
                    CreatedDate = DateTime.UtcNow,
                    IsDeleted = false,
                    BackendName = s.Backend,
                    NameAr = s.Ar,
                    NameEn = s.En,
                    DisplayOrder = s.Order
                });
            }
        }
        catch (Exception ex)
        {
            errors.Add(new ImportError("InsertSkillTypes", null, ex.GetBaseException().Message));
        }
    }

    // ================= Import Skills =================

    private static void ImportSkills(TawtheefDbContext db, List<ImportError> errors)
    {
        try
        {
            var skillTypeId = Guid.Parse("2F1B6CE7-CBC3-2B5C-B264-A02C6A87BE1D");
            var createdById = Guid.Parse("42E0D563-7603-453C-81B1-6B2325622B40");
            var now = DateTime.UtcNow;

            var skills = new (string Backend, string Ar, string En, string? DescAr, string? DescEn, int Order)[]
            {
                ("REQUIREMENTS_ANALYSIS", "تحليل المتطلبات", "Requirements Analysis",
                    "تحليل احتياجات النظام وتحويلها إلى متطلبات واضحة",
                    "Analyzing system needs and translating them into clear requirements", 1),
                ("SYSTEM_DESIGN", "تصميم النظام", "System Design",
                    "تصميم هيكلية النظام والمكونات الرئيسية",
                    "Designing system architecture and core components", 2),
                ("BACKEND_DEVELOPMENT", "برمجة الواجهة الخلفية", "Backend Development",
                    "تطوير منطق الأعمال والخدمات الخلفية",
                    "Developing business logic and backend services", 3),
                ("FRONTEND_DEVELOPMENT", "برمجة الواجهة الأمامية", "Frontend Development",
                    "تطوير واجهات المستخدم وتجربة الاستخدام",
                    "Developing user interfaces and user experience", 4),
                ("DATABASE_MANAGEMENT", "إدارة قواعد البيانات", "Database Management",
                    "تصميم وإدارة قواعد البيانات",
                    "Designing and managing databases", 5),
                ("SOFTWARE_TESTING", "اختبار البرمجيات", "Software Testing",
                    "اختبار جودة وأداء النظام",
                    "Testing system quality and performance", 6),
                ("SYSTEM_INTEGRATION", "تكامل الأنظمة", "System Integration",
                    "ربط الأنظمة والخدمات الخارجية",
                    "Integrating systems and external services", 7),
                ("INFORMATION_SECURITY", "أمن المعلومات", "Information Security",
                    "تأمين النظام وحماية البيانات",
                    "Securing the system and protecting data", 8),
                ("VERSION_MANAGEMENT", "إدارة الإصدارات", "Version Management",
                    "إدارة إصدارات النظام والتحديثات",
                    "Managing system versions and releases", 9),
                ("SYSTEM_SUPPORT", "دعم وصيانة النظام", "System Support & Maintenance",
                    "دعم النظام ومعالجة المشاكل",
                    "Supporting the system and handling issues", 10),
            };

            var existingBackends = new HashSet<string>(
                db.Skill.AsNoTracking().Select(x => x.BackendName),
                StringComparer.OrdinalIgnoreCase);

            foreach (var s in skills)
            {
                if (!existingBackends.Add(s.Backend))
                    continue;

                db.Skill.Add(new Skill
                {
                    Id = Guid.NewGuid(),
                    SkillTypeId = skillTypeId,
                    NameAr = s.Ar,
                    NameEn = s.En,
                    DescriptionAr = s.DescAr,
                    DescriptionEn = s.DescEn,
                    BackendName = s.Backend,
                    DisplayOrder = s.Order,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedById = createdById,
                    CreatedDate = now
                });
            }
        }
        catch (Exception ex)
        {
            errors.Add(new ImportError("InsertSkills", null, ex.GetBaseException().Message));
        }
    }

    // ================= Import MajorSkills =================

    private static void ImportMajorSkills(TawtheefDbContext db, List<ImportError> errors, int majorsCount = 10, int skillsCount = 10)
    {
        try
        {
            var createdById = AdminUserIds.Admin1UserId;
            var now = DateTime.UtcNow;

            var majorIds = db.Major.AsNoTracking()
                .OrderBy(m => m.DisplayOrder)
                .ThenBy(m => m.NameEn)
                .Select(m => m.Id)
                .Take(majorsCount)
                .ToList();

            var skillIds = db.Skill.AsNoTracking()
                .OrderBy(s => s.DisplayOrder)
                .ThenBy(s => s.NameEn)
                .Select(s => s.Id)
                .Take(skillsCount)
                .ToList();

            if (majorIds.Count == 0)
            {
                errors.Add(new ImportError("SeedMajorSkills", null, "No majors found to seed."));
                return;
            }

            if (skillIds.Count == 0)
            {
                errors.Add(new ImportError("SeedMajorSkills", null, "No skills found to seed."));
                return;
            }

            var existingLinks = new HashSet<(Guid MajorId, Guid SkillId)>(
                db.Set<MajorSkill>()
                    .AsNoTracking()
                    .Where(x => !x.IsDeleted && majorIds.Contains(x.MajorId) && skillIds.Contains(x.SkillId))
                    .Select(x => new ValueTuple<Guid, Guid>(x.MajorId, x.SkillId)));

            foreach (var majorId in majorIds)
            foreach (var skillId in skillIds.Where(skillId => existingLinks.Add((majorId, skillId))))
            {
                db.Set<MajorSkill>().Add(new MajorSkill
                {
                    Id = Guid.NewGuid(),
                    MajorId = majorId,
                    SkillId = skillId,
                    IsSkillRequired = false,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedById = createdById,
                    CreatedDate = now
                });
            }
        }
        catch (Exception ex)
        {
            errors.Add(new ImportError("SeedMajorSkills", null, ex.GetBaseException().Message));
        }
    }
}

#endregion

#region Error Model

public sealed record ImportError(string FileName, int? RowNumber, string Message);

#endregion
