using System.Globalization;
using CsvHelper;
using Seeds.Models;
using Seeds.Pipeline;
using Seeds.SeedData;

namespace Seeds.Validation;

internal sealed record SeedPreflightResult(
    IReadOnlyDictionary<string, int> RowCounts,
    IReadOnlyList<ImportError> Errors);

internal sealed record SeedCsvRow(int RowNumber, IReadOnlyDictionary<string, string> Values)
{
    public string Get(string column)
        => Values.TryGetValue(column, out var value) ? value.Trim() : string.Empty;
}

internal static class SeedPreflightValidator
{
    private static readonly System.Text.UTF8Encoding StrictUtf8 = new(
        encoderShouldEmitUTF8Identifier: false,
        throwOnInvalidBytes: true);

    public static SeedPreflightResult Validate(IReadOnlyList<ImportStep> selected)
    {
        var selectedCodes = new HashSet<string>(
            selected.Select(x => x.Code),
            StringComparer.OrdinalIgnoreCase);

        var errors = new List<ImportError>();
        var rowCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        List<SeedCsvRow>? countries = null;
        List<SeedCsvRow>? cities = null;
        List<SeedCsvRow>? majors = null;
        List<SeedCsvRow>? skillTypes = null;
        List<SeedCsvRow>? skills = null;

        if (selectedCodes.Contains("COUNTRY"))
        {
            countries = ReadCsv(
                "CountryData.csv",
                ["Id", "BackendName", "NameAr", "NameEn", "DisplayOrder", "Code"],
                errors,
                rowCounts);

            ValidateGuidColumn("CountryData.csv", countries, "Id", required: true, errors);
            ValidateIntColumn("CountryData.csv", countries, "DisplayOrder", required: true, errors);
            ValidateIntColumn("CountryData.csv", countries, "Code", required: true, errors);
            ValidateRequiredText("CountryData.csv", countries, ["BackendName", "NameAr", "NameEn"], errors);
            ValidateDuplicateGuid("CountryData.csv", countries, "Id", errors);
            ValidateDuplicateBackend(
                "CountryData.csv",
                countries,
                row => Truncate(row.Get("BackendName"), 50),
                errors);
        }

        if (selectedCodes.Contains("CITY"))
        {
            cities = ReadCsv(
                "CityData.csv",
                ["Id", "CountryId", "BackendName", "NameAr", "NameEn", "DisplayOrder"],
                errors,
                rowCounts);

            ValidateGuidColumn("CityData.csv", cities, "Id", required: true, errors);
            ValidateGuidColumn("CityData.csv", cities, "CountryId", required: true, errors);
            ValidateIntColumn("CityData.csv", cities, "DisplayOrder", required: true, errors);
            ValidateRequiredText("CityData.csv", cities, ["BackendName", "NameAr", "NameEn"], errors);
            ValidateDuplicateGuid("CityData.csv", cities, "Id", errors);
            ValidateDuplicateBackend(
                "CityData.csv",
                cities,
                row => SeedValueNormalizer.ToBackendName(row.Get("NameEn")),
                errors);

            if (countries is not null)
                ValidateForeignKey(
                    "CityData.csv",
                    cities,
                    "CountryId",
                    "CountryData.csv",
                    countries,
                    "Id",
                    errors);
        }

        if (selectedCodes.Contains("UNIV"))
        {
            List<SeedCsvRow> universities = ReadCsv(
                "UniversityData.csv",
                ["Id", "CityId", "NameAr", "NameEn", "DisplayOrder"],
                errors,
                rowCounts);

            ValidateGuidColumn("UniversityData.csv", universities, "Id", required: true, errors);
            ValidateGuidColumn("UniversityData.csv", universities, "CityId", required: true, errors);
            ValidateIntColumn("UniversityData.csv", universities, "DisplayOrder", required: true, errors);
            ValidateRequiredText("UniversityData.csv", universities, ["NameAr", "NameEn"], errors);
            ValidateDuplicateGuid("UniversityData.csv", universities, "Id", errors);
            ValidateDuplicateBackend(
                "UniversityData.csv",
                universities,
                row => SeedValueNormalizer.ToBackendName(row.Get("NameEn")),
                errors);

            if (cities is not null)
                ValidateForeignKey(
                    "UniversityData.csv",
                    universities,
                    "CityId",
                    "CityData.csv",
                    cities,
                    "Id",
                    errors);
        }

        if (selectedCodes.Contains("MAJOR"))
        {
            majors = ReadCsv(
                "MajorData.csv",
                ["Id", "BackendName", "NameAr", "NameEn", "DisplayOrder", "ParentId"],
                errors,
                rowCounts);

            ValidateGuidColumn("MajorData.csv", majors, "Id", required: true, errors);
            ValidateGuidColumn("MajorData.csv", majors, "ParentId", required: false, errors);
            ValidateIntColumn("MajorData.csv", majors, "DisplayOrder", required: true, errors);
            ValidateRequiredText("MajorData.csv", majors, ["BackendName", "NameAr", "NameEn"], errors);
            ValidateDuplicateGuid("MajorData.csv", majors, "Id", errors);
            ValidateDuplicateBackend(
                "MajorData.csv",
                majors,
                row => SeedValueNormalizer.ToBackendName(row.Get("BackendName")),
                errors);
            ValidateMajorHierarchy(majors, errors);
        }

        if (selectedCodes.Contains("JTITLE"))
        {
            var jobTitles = ReadCsv(
                "JobTitlesData.csv",
                ["JobNumber", "JobNameAr", "JobNameEn"],
                errors,
                rowCounts);

            ValidateRequiredText("JobTitlesData.csv", jobTitles, ["JobNumber", "JobNameAr", "JobNameEn"], errors);
            ValidateDuplicateText(
                "JobTitlesData.csv",
                jobTitles,
                "JobNumber",
                errors);
        }

        if (selectedCodes.Contains("SKTYPE"))
        {
            skillTypes = ReadCsv(
                "SkillTypeData.csv",
                ["Id", "BackendName", "NameAr", "NameEn", "DisplayOrder"],
                errors,
                rowCounts);

            ValidateGuidColumn("SkillTypeData.csv", skillTypes, "Id", required: true, errors);
            ValidateIntColumn("SkillTypeData.csv", skillTypes, "DisplayOrder", required: true, errors);
            ValidateRequiredText(
                "SkillTypeData.csv",
                skillTypes,
                ["BackendName", "NameAr", "NameEn"],
                errors);
            ValidateDuplicateGuid("SkillTypeData.csv", skillTypes, "Id", errors);
            ValidateDuplicateText("SkillTypeData.csv", skillTypes, "BackendName", errors);
        }

        if (selectedCodes.Contains("SKILL"))
        {
            skills = ReadCsv(
                "SkillData.csv",
                [
                    "Id",
                    "SkillTypeBackendName",
                    "BackendName",
                    "NameAr",
                    "NameEn",
                    "DisplayOrder",
                    "IsActive"
                ],
                errors,
                rowCounts);

            ValidateGuidColumn("SkillData.csv", skills, "Id", required: true, errors);
            ValidateIntColumn("SkillData.csv", skills, "DisplayOrder", required: true, errors);
            ValidateBoolColumn("SkillData.csv", skills, "IsActive", required: true, errors);
            ValidateRequiredText(
                "SkillData.csv",
                skills,
                ["SkillTypeBackendName", "BackendName", "NameAr", "NameEn"],
                errors);
            ValidateDuplicateGuid("SkillData.csv", skills, "Id", errors);
            ValidateDuplicateText("SkillData.csv", skills, "BackendName", errors);

            if (skillTypes is not null)
                ValidateTextForeignKey(
                    "SkillData.csv",
                    skills,
                    "SkillTypeBackendName",
                    "SkillTypeData.csv",
                    skillTypes,
                    "BackendName",
                    errors);
        }

        if (selectedCodes.Contains("MSLINK"))
        {
            var majorSkills = ReadCsv(
                "MajorSkillData.csv",
                ["Id", "MajorId", "SkillBackendName"],
                errors,
                rowCounts);

            ValidateGuidColumn("MajorSkillData.csv", majorSkills, "Id", required: true, errors);
            ValidateGuidColumn("MajorSkillData.csv", majorSkills, "MajorId", required: true, errors);
            ValidateRequiredText("MajorSkillData.csv", majorSkills, ["SkillBackendName"], errors);
            ValidateDuplicateGuid("MajorSkillData.csv", majorSkills, "Id", errors);
            ValidateDuplicateComposite(
                "MajorSkillData.csv",
                majorSkills,
                ["MajorId", "SkillBackendName"],
                errors);

            if (majors is not null)
                ValidateForeignKey(
                    "MajorSkillData.csv",
                    majorSkills,
                    "MajorId",
                    "MajorData.csv",
                    majors,
                    "Id",
                    errors);

            if (skills is not null)
                ValidateTextForeignKey(
                    "MajorSkillData.csv",
                    majorSkills,
                    "SkillBackendName",
                    "SkillData.csv",
                    skills,
                    "BackendName",
                    errors);
        }

        return new SeedPreflightResult(rowCounts, errors);
    }

    private static List<SeedCsvRow> ReadCsv(
        string fileName,
        IReadOnlyCollection<string> requiredColumns,
        List<ImportError> errors,
        IDictionary<string, int> rowCounts)
    {
        var path = SeedDataPath.Get(fileName);
        var rows = new List<SeedCsvRow>();

        if (!File.Exists(path))
        {
            errors.Add(new ImportError(fileName, null, $"Seed file not found: {path}"));
            rowCounts[fileName] = 0;
            return rows;
        }

        try
        {
            using var reader = new StreamReader(
                path,
                StrictUtf8,
                detectEncodingFromByteOrderMarks: true);

            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            if (!csv.Read())
            {
                errors.Add(new ImportError(fileName, null, "CSV file is empty."));
                rowCounts[fileName] = 0;
                return rows;
            }

            csv.ReadHeader();
            var headers = csv.HeaderRecord ?? [];

            foreach (var required in requiredColumns)
            {
                if (!headers.Contains(required, StringComparer.OrdinalIgnoreCase))
                    errors.Add(new ImportError(fileName, 1, $"Missing required column '{required}'."));
            }

            while (csv.Read())
            {
                var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                foreach (var header in headers)
                    values[header] = csv.GetField(header) ?? string.Empty;

                rows.Add(new SeedCsvRow(csv.Context.Parser?.Row ?? 0, values));
            }
        }
        catch (System.Text.DecoderFallbackException ex)
        {
            errors.Add(new ImportError(
                fileName,
                null,
                $"File is not valid UTF-8. Convert it to UTF-8 before seeding. {ex.Message}"));
        }
        catch (Exception ex)
        {
            errors.Add(new ImportError(fileName, null, $"Unable to read CSV: {ex.GetBaseException().Message}"));
        }

        rowCounts[fileName] = rows.Count;
        return rows;
    }

    private static void ValidateGuidColumn(
        string fileName,
        IEnumerable<SeedCsvRow> rows,
        string column,
        bool required,
        List<ImportError> errors)
    {
        foreach (var row in rows)
        {
            var value = row.Get(column);
            if (string.IsNullOrWhiteSpace(value))
            {
                if (required)
                    errors.Add(new ImportError(fileName, row.RowNumber, $"{column} is required."));

                continue;
            }

            if (!Guid.TryParse(value, out _))
                errors.Add(new ImportError(fileName, row.RowNumber, $"Invalid {column}: '{value}'."));
        }
    }

    private static void ValidateIntColumn(
        string fileName,
        IEnumerable<SeedCsvRow> rows,
        string column,
        bool required,
        List<ImportError> errors)
    {
        foreach (var row in rows)
        {
            var value = row.Get(column);
            if (string.IsNullOrWhiteSpace(value))
            {
                if (required)
                    errors.Add(new ImportError(fileName, row.RowNumber, $"{column} is required."));

                continue;
            }

            if (!int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out _))
                errors.Add(new ImportError(fileName, row.RowNumber, $"Invalid {column}: '{value}'."));
        }
    }

    private static void ValidateBoolColumn(
        string fileName,
        IEnumerable<SeedCsvRow> rows,
        string column,
        bool required,
        List<ImportError> errors)
    {
        foreach (var row in rows)
        {
            var value = row.Get(column);
            if (string.IsNullOrWhiteSpace(value))
            {
                if (required)
                    errors.Add(new ImportError(fileName, row.RowNumber, $"{column} is required."));

                continue;
            }

            if (!bool.TryParse(value, out _))
                errors.Add(new ImportError(fileName, row.RowNumber, $"Invalid {column}: '{value}'."));
        }
    }

    private static void ValidateRequiredText(
        string fileName,
        IEnumerable<SeedCsvRow> rows,
        IReadOnlyCollection<string> columns,
        List<ImportError> errors)
    {
        foreach (var row in rows)
        foreach (var column in columns)
        {
            if (string.IsNullOrWhiteSpace(row.Get(column)))
                errors.Add(new ImportError(fileName, row.RowNumber, $"{column} is required."));
        }
    }

    private static void ValidateDuplicateGuid(
        string fileName,
        IEnumerable<SeedCsvRow> rows,
        string column,
        List<ImportError> errors)
    {
        var seen = new Dictionary<Guid, int>();

        foreach (var row in rows)
        {
            if (!Guid.TryParse(row.Get(column), out var id))
                continue;

            if (seen.TryGetValue(id, out var firstRow))
                errors.Add(new ImportError(
                    fileName,
                    row.RowNumber,
                    $"Duplicate {column}: {id}. First occurrence at row {firstRow}."));
            else
                seen[id] = row.RowNumber;
        }
    }

    private static void ValidateDuplicateText(
        string fileName,
        IEnumerable<SeedCsvRow> rows,
        string column,
        List<ImportError> errors)
    {
        var seen = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        foreach (var row in rows)
        {
            var value = row.Get(column);
            if (string.IsNullOrWhiteSpace(value))
                continue;

            if (seen.TryGetValue(value, out var firstRow))
                errors.Add(new ImportError(
                    fileName,
                    row.RowNumber,
                    $"Duplicate {column}: '{value}'. First occurrence at row {firstRow}."));
            else
                seen[value] = row.RowNumber;
        }
    }

    private static void ValidateDuplicateBackend(
        string fileName,
        IEnumerable<SeedCsvRow> rows,
        Func<SeedCsvRow, string> selector,
        List<ImportError> errors)
    {
        var seen = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        foreach (var row in rows)
        {
            var backend = selector(row);
            if (string.IsNullOrWhiteSpace(backend))
            {
                errors.Add(new ImportError(
                    fileName,
                    row.RowNumber,
                    "Effective BackendName is empty after normalization."));
                continue;
            }

            if (seen.TryGetValue(backend, out var firstRow))
                errors.Add(new ImportError(
                    fileName,
                    row.RowNumber,
                    $"Duplicate effective BackendName: '{backend}'. First occurrence at row {firstRow}."));
            else
                seen[backend] = row.RowNumber;
        }
    }

    private static void ValidateForeignKey(
        string childFile,
        IEnumerable<SeedCsvRow> children,
        string childForeignKey,
        string parentFile,
        IEnumerable<SeedCsvRow> parents,
        string parentKey,
        List<ImportError> errors)
    {
        var parentIds = new HashSet<Guid>(
            parents
                .Select(x => Guid.TryParse(x.Get(parentKey), out var id) ? id : Guid.Empty)
                .Where(x => x != Guid.Empty));

        foreach (var child in children)
        {
            if (!Guid.TryParse(child.Get(childForeignKey), out var foreignKey))
                continue;

            if (!parentIds.Contains(foreignKey))
                errors.Add(new ImportError(
                    childFile,
                    child.RowNumber,
                    $"{childForeignKey} '{foreignKey}' does not exist in {parentFile}."));
        }
    }

    private static void ValidateTextForeignKey(
        string childFile,
        IEnumerable<SeedCsvRow> children,
        string childForeignKey,
        string parentFile,
        IEnumerable<SeedCsvRow> parents,
        string parentKey,
        List<ImportError> errors)
    {
        var parentValues = new HashSet<string>(
            parents
                .Select(x => x.Get(parentKey))
                .Where(x => !string.IsNullOrWhiteSpace(x)),
            StringComparer.OrdinalIgnoreCase);

        foreach (var child in children)
        {
            var foreignKey = child.Get(childForeignKey);
            if (string.IsNullOrWhiteSpace(foreignKey))
                continue;

            if (!parentValues.Contains(foreignKey))
                errors.Add(new ImportError(
                    childFile,
                    child.RowNumber,
                    $"{childForeignKey} '{foreignKey}' does not exist in {parentFile}."));
        }
    }

    private static void ValidateDuplicateComposite(
        string fileName,
        IEnumerable<SeedCsvRow> rows,
        IReadOnlyList<string> columns,
        List<ImportError> errors)
    {
        var seen = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        foreach (var row in rows)
        {
            var values = columns.Select(row.Get).ToArray();
            if (values.Any(string.IsNullOrWhiteSpace))
                continue;

            var key = string.Join("\u001F", values);

            if (seen.TryGetValue(key, out var firstRow))
                errors.Add(new ImportError(
                    fileName,
                    row.RowNumber,
                    $"Duplicate combination ({string.Join(", ", columns)}). First occurrence at row {firstRow}."));
            else
                seen[key] = row.RowNumber;
        }
    }

    private static void ValidateMajorHierarchy(
        IReadOnlyList<SeedCsvRow> rows,
        List<ImportError> errors)
    {
        var byId = new Dictionary<Guid, SeedCsvRow>();

        foreach (var row in rows)
        {
            if (Guid.TryParse(row.Get("Id"), out var id))
                byId.TryAdd(id, row);
        }

        foreach (var row in rows)
        {
            if (!Guid.TryParse(row.Get("Id"), out var id))
                continue;

            var parentText = row.Get("ParentId");
            if (string.IsNullOrWhiteSpace(parentText) || !Guid.TryParse(parentText, out var parentId))
                continue;

            if (id == parentId)
            {
                errors.Add(new ImportError(
                    "MajorData.csv",
                    row.RowNumber,
                    $"Major '{id}' cannot reference itself as ParentId."));
                continue;
            }

            if (!byId.ContainsKey(parentId))
                errors.Add(new ImportError(
                    "MajorData.csv",
                    row.RowNumber,
                    $"ParentId '{parentId}' does not exist in MajorData.csv."));
        }

        var globallyChecked = new HashSet<Guid>();

        foreach (var start in byId.Keys)
        {
            if (globallyChecked.Contains(start))
                continue;

            var path = new Dictionary<Guid, int>();
            var current = start;
            var depth = 0;

            while (true)
            {
                if (path.ContainsKey(current))
                {
                    var cycleRow = byId[current];

                    errors.Add(new ImportError(
                        "MajorData.csv",
                        cycleRow.RowNumber,
                        $"Circular Major hierarchy detected at Id '{current}'."));

                    break;
                }

                if (globallyChecked.Contains(current))
                    break;

                path[current] = depth++;

                var currentRow = byId[current];
                var parentText = currentRow.Get("ParentId");

                if (string.IsNullOrWhiteSpace(parentText) ||
                    !Guid.TryParse(parentText, out var parentId) ||
                    !byId.ContainsKey(parentId))
                    break;

                current = parentId;
            }

            foreach (var id in path.Keys)
                globallyChecked.Add(id);
        }
    }

    private static string Truncate(string value, int maxLength)
        => value.Length <= maxLength ? value : value[..maxLength];
}
