namespace Seeds.Pipeline;

internal static class ImportCatalog
{
    public static readonly List<ImportStep> All =
    [
        new ImportStep { Code = "COUNTRY", Title = "Import Countries (CSV)", RunAsync = (r, ct) => r.ImportCountriesAsync(ct) },
        new ImportStep { Code = "CITY",    Title = "Import Cities (CSV)", DependsOn = ["COUNTRY"], RunAsync = (r, ct) => r.ImportCitiesAsync(ct) },
        new ImportStep { Code = "UNIV",    Title = "Import Universities (CSV)", DependsOn = ["CITY"], RunAsync = (r, ct) => r.ImportUniversitiesAsync(ct) },
        new ImportStep { Code = "MAJOR",   Title = "Import Majors (CSV)",    RunAsync = (r, ct) => r.ImportMajorsAsync(ct) },
        new ImportStep { Code = "JTITLE",  Title = "Import Job Titles (CSV)", RunAsync = (r, ct) => r.ImportJobTitlesAsync(ct) },
        new ImportStep { Code = "SKTYPE",  Title = "Import Skill Types (CSV)", RunAsync = (r, ct) => r.ImportSkillTypesAsync(ct) },
        new ImportStep { Code = "SKILL",   Title = "Import Skills (CSV)", DependsOn = ["SKTYPE"], RunAsync = (r, ct) => r.ImportSkillsAsync(ct) },
        new ImportStep { Code = "MSLINK",  Title = "Import Major-Skill Links (CSV)", DependsOn = ["MAJOR", "SKILL"], RunAsync = (r, ct) => r.ImportMajorSkillsAsync(ct) },
        new ImportStep { Code = "QBLOOKUPS", Title = "Seed Question Bank Lookups", RunAsync = (r, ct) => r.ImportQuestionBankLookupsAsync(ct) },
        new ImportStep
        {
            Code = "APPLICANTS",
            Title = "Seed 10k complete Applicant Profiles (all types/statuses)",
            DependsOn = ["COUNTRY", "UNIV", "MAJOR", "JTITLE", "MSLINK"],
            UseTransaction = false,
            RunAsync = (r, ct) => r.SeedApplicantsAsync(ct)
        },
    ];

    public static List<ImportStep> ExpandDependencies(IReadOnlyList<ImportStep> selected)
    {
        var selectedCodes = new HashSet<string>(selected.Select(x => x.Code), StringComparer.OrdinalIgnoreCase);
        var byCode = All.ToDictionary(x => x.Code, StringComparer.OrdinalIgnoreCase);

        var changed = true;
        while (changed)
        {
            changed = false;
            foreach (var step in All.Where(step => selectedCodes.Contains(step.Code)))
            foreach (var dependency in step.DependsOn)
            {
                if (!byCode.ContainsKey(dependency))
                    throw new InvalidOperationException($"Unknown seed dependency '{dependency}' for '{step.Code}'.");

                changed |= selectedCodes.Add(dependency);
            }
        }

        // Topologically sort instead of relying on the display order of All. This keeps
        // individual selections and "ALL" safe when the catalog is rearranged later.
        var result = new List<ImportStep>(selectedCodes.Count);
        var visiting = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        void Visit(string code)
        {
            if (visited.Contains(code)) return;
            if (!visiting.Add(code))
                throw new InvalidOperationException($"Circular seed dependency detected at '{code}'.");

            var step = byCode[code];
            foreach (var dependency in step.DependsOn)
                Visit(dependency);

            visiting.Remove(code);
            visited.Add(code);
            result.Add(step);
        }

        // All provides the stable ordering for otherwise independent steps.
        foreach (var step in All.Where(x => selectedCodes.Contains(x.Code)))
            Visit(step.Code);

        return result;
    }
}
