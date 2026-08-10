using Microsoft.EntityFrameworkCore;
using Seeds.Models;
using Tawtheef.Infrastructure.Data;

namespace Seeds.Importing;

internal sealed class SeedImportContext(
    TawtheefDbContext db,
    List<ImportError> errors)
{
    private LookupSeedState? _countries;
    private LookupSeedState? _cities;
    private LookupSeedState? _universities;
    private LookupSeedState? _majors;

    internal TawtheefDbContext Db { get; } = db;

    public List<ImportError> Errors { get; } = errors;

    public LookupSeedState Countries =>
        _countries ??= new LookupSeedState(
            Db.Country
                .AsNoTracking()
                .Select(x => x.Id),
            Db.Country
                .AsNoTracking()
                .Where(x => x.BackendName != null)
                .Select(x => x.BackendName!));

    public LookupSeedState Cities =>
        _cities ??= new LookupSeedState(
            Db.City
                .AsNoTracking()
                .Select(x => x.Id),
            Db.City
                .AsNoTracking()
                .Where(x => x.BackendName != null)
                .Select(x => x.BackendName!));

    public LookupSeedState Universities =>
        _universities ??= new LookupSeedState(
            Db.University
                .AsNoTracking()
                .Select(x => x.Id),
            Db.University
                .AsNoTracking()
                .Where(x => x.BackendName != null)
                .Select(x => x.BackendName!));

    public LookupSeedState Majors =>
        _majors ??= new LookupSeedState(
            Db.Major
                .AsNoTracking()
                .Select(x => x.Id),
            Db.Major
                .AsNoTracking()
                .Where(x => x.BackendName != null)
                .Select(x => x.BackendName!));
}

internal sealed class LookupSeedState(
    IEnumerable<Guid> ids,
    IEnumerable<string> backendNames)
{
    private readonly HashSet<Guid> _ids = [..ids];
    private readonly HashSet<string> _backendNames = new(
        backendNames,
        StringComparer.OrdinalIgnoreCase);

    public bool ContainsId(Guid id)
        => _ids.Contains(id);

    public bool TryRegisterId(
        Guid id,
        List<ImportError> errors,
        string fileName,
        int? rowNumber)
    {
        if (_ids.Add(id))
            return true;

        errors.Add(new ImportError(
            fileName,
            rowNumber,
            $"Duplicate Id: {id}"));

        return false;
    }

    public bool TryRegisterBackend(
        string backendName,
        List<ImportError> errors,
        string fileName,
        int? rowNumber)
    {
        if (string.IsNullOrWhiteSpace(backendName))
            return true;

        if (_backendNames.Add(backendName))
            return true;

        errors.Add(new ImportError(
            fileName,
            rowNumber,
            $"Duplicate BackendName: {backendName}"));

        return false;
    }
}
