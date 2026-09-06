using Seeds.Importing.Applicants;
using Seeds.Importing.Lookups;
using Seeds.Models;
using Tawtheef.Infrastructure.Data;

namespace Seeds.Importing;

internal sealed class ImportRunner(
    TawtheefDbContext db,
    List<ImportError> errors)
{
    private readonly SeedImportContext _context = new(
        db,
        errors);

    public Task ImportCountriesAsync(
        CancellationToken ct)
        => CountryImporter.ImportAsync(
            _context,
            ct);

    public Task ImportCitiesAsync(
        CancellationToken ct)
        => CityImporter.ImportAsync(
            _context,
            ct);

    public Task ImportUniversitiesAsync(
        CancellationToken ct)
        => UniversityImporter.ImportAsync(
            _context,
            ct);

    public Task ImportMajorsAsync(
        CancellationToken ct)
        => MajorImporter.ImportAsync(
            _context,
            ct);

    public Task ImportJobTitlesAsync(
        CancellationToken ct)
        => JobTitleImporter.ImportAsync(
            _context,
            ct);

    public Task ImportSkillTypesAsync(
        CancellationToken ct)
        => SkillTypeImporter.ImportAsync(
            _context,
            ct);

    public Task ImportSkillsAsync(
        CancellationToken ct)
        => SkillImporter.ImportAsync(
            _context,
            ct);

    public Task ImportMajorSkillsAsync(
        CancellationToken ct)
        => MajorSkillImporter.ImportAsync(
            _context,
            ct);

    public Task ImportQuestionBankLookupsAsync(
        CancellationToken ct)
        => QuestionBankLookupImporter.ImportAsync(
            _context,
            ct);

    public Task SeedApplicantsAsync(
        CancellationToken ct)
        => ApplicantImporter.ImportAsync(
            _context,
            ct);
}
