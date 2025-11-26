using CsvHelper;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;
using Tawtheef.Infrastructure.Data;
using Tawtheef.Infrastructure.Services.Identity;

Console.WriteLine("=== Import Started ===");
const string ConnectionString = "Server=DCDCSQL2DNET01;Database=Tawthef;Trust Server Certificate=true;User id=Sch_T; Password=Abc@1234;";
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var host = Host.CreateDefaultBuilder()
    .UseSerilog() // ensure Serilog packages are available
    .ConfigureServices((context, services) =>
    {
        services.AddSingleton(Log.Logger);
        services.AddScoped<CurrentUserService>();
        services.AddDbContext<TawtheefDbContext>(options => options.UseSqlServer(ConnectionString));
    })
    .Build();

using var scope = host.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<TawtheefDbContext>();

ImportCountries(db);
ImportCities(db);
ImportMajors(db);
ImportUniversities(db);

await db.SaveChangesAsync();

Console.WriteLine("=== Import Finished ===");
static string GetDataPath(string fileName)
{
    var basePath = AppContext.BaseDirectory; // bin/Debug/net8.0
    return Path.Combine(basePath, "SeedData", fileName);
}
static string ToBackendName(string? nameEn)
{
    if (string.IsNullOrWhiteSpace(nameEn))
        return string.Empty;

    // Remove special characters
    var cleaned = new string(nameEn
        .Where(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c))
        .ToArray());

    // Split into words
    var words = cleaned
        .Split(' ', StringSplitOptions.RemoveEmptyEntries)
        .Select(w => char.ToUpper(w[0]) + w.Substring(1).ToLower())
        .ToArray();

    // max length is 50 chars
    return string.Join("", words).Substring(0, Math.Min(50, string.Join("", words).Length));
}

static void ImportCountries(TawtheefDbContext db)
{
    var path = GetDataPath("CountryData.csv");
    using var reader = new StreamReader(path);
    using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
    var rows = csv.GetRecords<dynamic>().ToList();

    foreach (var entity in rows.Select(r => new Country
             {
                 Id = Guid.Parse(r.Id),
                 BackendName = ((string)r.BackendName).Substring(0, Math.Min(50, r.BackendName.Length)),
                 NameAr = r.NameAr,
                 NameEn = r.NameEn,
                 DescriptionAr = r.DescriptionAr,
                 DescriptionEn = r.DescriptionEn,
                 DisplayOrder = int.Parse(r.DisplayOrder),
                 Code = int.Parse(r.Code),
                 ISOCode = r.ISOCode ?? "",
                 CodeAlpha = r.CodeAlpha ?? ""
             }))
    {
        db.Country.Add(entity);
    }
}
static void ImportCities(TawtheefDbContext db)
{
    var path = GetDataPath("CityData.csv");
    using var reader = new StreamReader(path);
    using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
    var rows = csv.GetRecords<dynamic>().ToList();

    foreach (var entity in rows.Select(r => new City
             {
                 Id = Guid.Parse(r.Id),
                 BackendName = ToBackendName(r.NameEn),
                 NameAr = r.NameAr,
                 NameEn = r.NameEn,
                 DescriptionAr = r.DescriptionAr,
                 DescriptionEn = r.DescriptionEn,
                 DisplayOrder = int.Parse(r.DisplayOrder),
                 CountryId = Guid.Parse(r.CountryId),
                 Code = r.BackendName // or slugify
             }))
    {
        db.City.Add(entity);
    }
}
static void ImportMajors(TawtheefDbContext db)
{
    var path = GetDataPath("MajorData.csv");
    using var reader = new StreamReader(path);
    using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
    var rows = csv.GetRecords<dynamic>().ToList();

    foreach (var entity in from r in rows let parent = string.IsNullOrWhiteSpace(r.ParentId)
                 ? null
                 : Guid.Parse(r.ParentId) select new Major
             {
                 Id = Guid.Parse(r.Id),
                 BackendName = ToBackendName(r.NameEn),
                 NameAr = r.NameAr,
                 NameEn = r.NameEn,
                 DescriptionAr = r.DescriptionAr,
                 DescriptionEn = r.DescriptionEn,
                 DisplayOrder = int.Parse(r.DisplayOrder),
                 ParentId = (Guid?)parent
             })
    {
        db.Major.Add(entity);
    }
}
static void ImportUniversities(TawtheefDbContext db)
{
    var path = GetDataPath("UniversityData.csv");
    using var reader = new StreamReader(path);
    using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
    var rows = csv.GetRecords<dynamic>().ToList();

    foreach (var entity in rows.Select(r => new University
             {
                 Id = Guid.Parse(r.Id),
                 BackendName = ToBackendName(r.NameEn),
                 NameAr = r.NameAr,
                 NameEn = r.NameEn,
                 DescriptionAr = r.DescriptionAr,
                 DescriptionEn = r.DescriptionEn,
                 DisplayOrder = int.Parse(r.DisplayOrder),
                 CityId = Guid.Parse(r.CityId),
                 WebSite = r.Website,
                 Phone = r.Phone,
                 Email = r.Email,
                 Code = r.Code,
                 LogoEn = r.LogoEn,
                 LogoAr = r.LogoAr,
                 OriginalName = r.OriginalName
             }))
    {
        db.University.Add(entity);
    }
}
