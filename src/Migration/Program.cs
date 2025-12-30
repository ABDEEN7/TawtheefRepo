using System.Globalization;
using CsvHelper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Infrastructure.Data;
using Tawtheef.Infrastructure.Services.Identity;

namespace Migration;

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("=== Import Started ===");

        const string ConnectionString =
            "Server=DCDCSQL2DNET01;Database=Tawthef;Trust Server Certificate=true;User id=Sch_T; Password=Abc@1234;";
            //"Server=(localdb)\\MSSQLLocalDB;Database=TawtheefDB;Trusted_Connection=True;";

        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();

        var host = Host.CreateDefaultBuilder(args)
            .UseSerilog()
            .ConfigureServices((_, services) =>
            {
                services.AddSingleton(Log.Logger);
                services.AddDbContext<TawtheefDbContext>(options =>
                    options.UseSqlServer(ConnectionString));
                services.AddScoped<CurrentUserService>();
            })
            .Build();

        using var scope = host.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TawtheefDbContext>();

        var errors = new List<ImportError>();

        // ====== Pre-load existing data from DB ======
        var countryIds = new HashSet<Guid>(db.Country.Select(x => x.Id));
        var countryBackendNames = new HashSet<string>(db.Country.Select(x => x.BackendName));

        var cityIds = new HashSet<Guid>(db.City.Select(x => x.Id));
        var cityBackendNames = new HashSet<string>(db.City.Select(x => x.BackendName));

        var universityIds = new HashSet<Guid>(db.University.Select(x => x.Id));
        var universityBackendNames = new HashSet<string>(db.University.Select(x => x.BackendName));

        // ====== Import with validation ======
        ImportCountries(db, errors, countryIds, countryBackendNames);
        ImportCities(db, errors, cityIds, cityBackendNames, countryIds);
        ImportUniversities(db, errors, universityIds, universityBackendNames, cityIds);
        ImportMajors(db, errors);
        ImportOffices(db, errors);
        ImportSkillTypes(db, errors);

        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            Log.Error(ex, "SaveChanges failed");
            errors.Add(new ImportError("DB", 0, ex.GetBaseException().Message));
        }

        Console.WriteLine("=== Import Finished ===");

        if (errors.Any())
        {
            Console.WriteLine("=== ERRORS SUMMARY ===");
            foreach (var e in errors)
            {
                Console.WriteLine($"{e.FileName} - Row {e.RowNumber}: {e.Message}");
            }
        }
        else
        {
            Console.WriteLine("No errors 🎉");
        }
    }

    // ================= Helpers =================

    static string GetDataPath(string fileName)
    {
        var basePath = AppContext.BaseDirectory;
        return Path.Combine(basePath, "SeedData", fileName);
    }

    static string ToBackendName(string? nameEn)
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

    // ================ Import Countries =================

    static void ImportCountries(
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

                // Duplicate Id
                if (!existingIds.Add(id))
                {
                    errors.Add(new ImportError("CountryData.csv", row, $"Duplicate Id: {id}"));
                    continue;
                }

                // Duplicate BackendName
                if (!string.IsNullOrWhiteSpace(backendName) &&
                    !backendNames.Add(backendName))
                {
                    errors.Add(new ImportError("CountryData.csv", row, $"Duplicate BackendName: {backendName}"));
                    continue;
                }

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

    // ================ Import Cities =================

    static void ImportCities(
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

                // Duplicate Id
                if (!existingIds.Add(id))
                {
                    errors.Add(new ImportError("CityData.csv", row, $"Duplicate Id: {id}"));
                    continue;
                }

                // Duplicate BackendName
                if (!string.IsNullOrWhiteSpace(backendName) &&
                    !backendNames.Add(backendName))
                {
                    errors.Add(new ImportError("CityData.csv", row, $"Duplicate BackendName: {backendName}"));
                    continue;
                }

                // FK check: CountryId must exist
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

    // ================ Import Universities =================

    static void ImportUniversities(
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

                // Duplicate Id
                if (!existingIds.Add(id))
                {
                    errors.Add(new ImportError("UniversityData.csv", row, $"Duplicate Id: {id}"));
                    continue;
                }

                // Duplicate BackendName
                if (!string.IsNullOrWhiteSpace(backendName) &&
                    !backendNames.Add(backendName))
                {
                    errors.Add(new ImportError("UniversityData.csv", row, $"Duplicate BackendName: {backendName}"));
                    continue;
                }

                // FK check: CityId must exist
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
                    LogoEn = r.LogoEn,
                    LogoAr = r.LogoAr,
                    OriginalName = r.OriginalName
                });
            }
            catch (Exception ex)
            {
                errors.Add(new ImportError("UniversityData.csv", row, ex.GetBaseException().Message));
            }
        }
    }

    // ================ Import Majors =================

    static void ImportMajors(TawtheefDbContext db, List<ImportError> errors)
    {
        var path = GetDataPath("MajorData.csv");
        using var reader = new StreamReader(path);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        var rows = new List<dynamic>();

        while (csv.Read())
        {
            rows.Add(csv.GetRecord<dynamic>());
        }

        // Keep track of processed items
        var remaining = rows.ToList();
        var inserted = new HashSet<Guid>();

        int safetyCounter = 0;
        while (remaining.Any() && safetyCounter < 10000) // protect against infinite loop
        {
            safetyCounter++;

            var toInsert = remaining
                .Where(r =>
                {
                    string parentIdStr = r.ParentId;
                    if (string.IsNullOrWhiteSpace(parentIdStr))
                        return true; // root item (insert now)

                    var parentId = Guid.Parse(parentIdStr);
                    return inserted.Contains(parentId); // parent already inserted
                })
                .ToList();

            if (!toInsert.Any())
            {
                // No progress — break and report remaining as errors
                foreach (var r in remaining)
                {
                    var rowNumber = csv.Context.Parser?.Row;
                    errors.Add(new ImportError("MajorData.csv", rowNumber,
                        $"Parent not found for child: {r.Id} | Parent: {r.ParentId}"));
                }

                break;
            }

            foreach (var r in toInsert)
            {
                var rowNumber = csv.Context.Parser?.Row;
                try
                {
                    Guid? parent = string.IsNullOrWhiteSpace(r.ParentId) ? null : Guid.Parse(r.ParentId);

                    var entity = new Major
                    {
                        Id = Guid.Parse((string)r.Id),
                        BackendName = ToBackendName((string)r.BackendName),
                        NameAr = r.NameAr,
                        NameEn = r.NameEn,
                        DescriptionAr = r.DescriptionAr,
                        DescriptionEn = r.DescriptionEn,
                        DisplayOrder = int.Parse((string)r.DisplayOrder),
                        ParentId = parent
                    };

                    db.Major.Add(entity);
                    inserted.Add(entity.Id);
                }
                catch (Exception ex)
                {
                    errors.Add(new ImportError("MajorData.csv", rowNumber, ex.GetBaseException().Message));
                }

                remaining.Remove(r);
            }
        }
    }

    // ================= Import Offices =================
     static void ImportOffices(TawtheefDbContext db, List<ImportError> errors)
     {
         try
         {
             var jordanId = Guid.Parse("b7f89fce-4f81-464a-9e99-7fc7c8bd1d54");
             var syriaId = Guid.Parse("21d0a391-7e7f-4a3e-bd27-9c26345c7e09");
             var ukId = Guid.Parse("cba347b0-123e-4eb1-91be-ca3a2725bbeb");
    
             var offices = new (Guid CountryId,string Code, string BackendName, string Ar, string En, int Order)[]
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
    
             foreach (var o in offices)
                 if (!db.Office.Any(x => x.BackendName == o.BackendName))
                     db.Office.Add(new Office
                     {
                         Id = Guid.NewGuid(),
                         CreatedDate = DateTimeOffset.UtcNow,
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
         catch (Exception ex)
         {
             errors.Add(new ImportError("InsertOffices", null, ex.GetBaseException().Message));
         }
    }


// ================= Import SkillTypes =================
    static void ImportSkillTypes(TawtheefDbContext db, List<ImportError> errors)
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

            foreach (var s in skills)
            {
                if (db.SkillType.Any(x => x.BackendName == s.Backend))
                    continue;

                db.SkillType.Add(new SkillType
                {
                    Id = Guid.NewGuid(),
                    CreatedDate = DateTimeOffset.UtcNow,
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
}

// ================ Error Model =================

public record ImportError(string FileName, int? RowNumber, string Message);
