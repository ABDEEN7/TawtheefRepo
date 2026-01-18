namespace Tawtheef.Application.Common;

public sealed record Translation(string Key, string Value);

public static class Translations
{
    private static readonly HashSet<Translation> English =
    [
        new ("CANDIDATE_NAME", "Candidate Name"),
        new ("CANDIDATE", "Candidate"),
        new ("DEPARTMENT", "Department"),
        new ("JOB_CATEGORY", "Job Category"),
        new ("CANDIDATE_CATEGORY", "Candidate Category"),
        new ("MAJOR", "Major"),
        new ("GENDER", "Gender"),
        new ("POINTS", "Points")
    ];

    private static readonly HashSet<Translation> Arabic =
    [
        new("CANDIDATE_NAME", "اسم المرشح"),
        new("CANDIDATE", "مرشح"),
        new("DEPARTMENT", "القسم"),
        new("JOB_CATEGORY", "فئة الوظيفة"),
        new("CANDIDATE_CATEGORY", "فئة المرشح"),
        new("MAJOR", "التخصص"),
        new("GENDER", "الجنس"),
        new("POINTS", "النقاط")
    ];

    public static string Get(string key, string language)
    {
        var set = language == "ar" ? Arabic : English;

        return set.FirstOrDefault(t => t.Key == key)?.Value ?? key;
    }
}