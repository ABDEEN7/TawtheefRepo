using System.Globalization;
using System.Text.Json;

namespace Tawtheef.Notifications.TemplateTester;

internal static class ModelBuilder
{
    public static object BuildModelFast(string templateKey, Type modelType, TesterState state)
    {
        Console.WriteLine();
        Console.WriteLine("Model mode: auto | last | prompt | json");
        var mode = Prompting.Prompt("Choose model mode", "auto", required: true).ToLowerInvariant();

        return mode switch
        {
            "auto" => BuildModelAuto(templateKey, modelType, state),
            "last" => BuildModelFromLast(templateKey, modelType, state),
            "prompt" => BuildModelPrompted(templateKey, modelType, state),
            "json" => BuildModelFromJson(modelType),
            _ => BuildModelAuto(templateKey, modelType, state)
        };
    }

    private static object BuildModelFromLast(string templateKey, Type modelType, TesterState state)
    {
        if (!state.LastModelValuesByTemplate.TryGetValue(templateKey, out var last) || last.Count == 0)
        {
            Console.WriteLine("No saved values for this template. Falling back to auto.");
            return BuildModelAuto(templateKey, modelType, state);
        }

        return BuildModelFromDictionary(modelType, last, templateKey, state);
    }

    private static object BuildModelAuto(string templateKey, Type modelType, TesterState state)
    {
        var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        var ctor = modelType.GetConstructors()
                       .OrderByDescending(c => c.GetParameters().Length)
                       .FirstOrDefault()
                   ?? throw new InvalidOperationException($"No public constructor found for {modelType.FullName}.");

        foreach (var p in ctor.GetParameters())
            dict[p.Name!] = GenerateSampleValue(p.ParameterType, p.Name!);

        return BuildModelFromDictionary(modelType, dict, templateKey, state);
    }

    private static object BuildModelPrompted(string templateKey, Type modelType, TesterState state)
    {
        var ctor = modelType.GetConstructors()
                       .OrderByDescending(c => c.GetParameters().Length)
                       .FirstOrDefault()
                   ?? throw new InvalidOperationException($"No public constructor found for {modelType.FullName}.");

        state.LastModelValuesByTemplate.TryGetValue(templateKey, out var last);
        last ??= new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var p in ctor.GetParameters())
        {
            var name = p.Name!;
            var typeName = Nullable.GetUnderlyingType(p.ParameterType)?.Name ?? p.ParameterType.Name;

            last.TryGetValue(name, out var lastVal);
            var input = Prompting.Prompt($"{name} ({typeName})", lastVal, required: false);

            if (string.IsNullOrWhiteSpace(input))
                input = GenerateSampleValue(p.ParameterType, name);

            dict[name] = input;
        }

        return BuildModelFromDictionary(modelType, dict, templateKey, state);
    }

    private static object BuildModelFromDictionary(
        Type modelType,
        Dictionary<string, string> dict,
        string templateKey,
        TesterState state)
    {
        var ctor = modelType.GetConstructors()
                       .OrderByDescending(c => c.GetParameters().Length)
                       .FirstOrDefault()
                   ?? throw new InvalidOperationException($"No public constructor found for {modelType.FullName}.");

        var parameters = ctor.GetParameters();
        var args = new object?[parameters.Length];

        for (var i = 0; i < parameters.Length; i++)
        {
            var p = parameters[i];
            var name = p.Name!;
            dict.TryGetValue(name, out var raw);
            raw ??= "";
            args[i] = ConvertInputSafe(raw, p.ParameterType, name);
        }

        state.LastModelValuesByTemplate[templateKey] = new Dictionary<string, string>(dict, StringComparer.OrdinalIgnoreCase);
        StateStore.Save(state);

        return ctor.Invoke(args);
    }

    private static object BuildModelFromJson(Type modelType)
    {
        Console.WriteLine("Paste JSON (end input with an empty line):");
        var lines = new List<string>();
        while (true)
        {
            var line = Console.ReadLine();
            if (string.IsNullOrEmpty(line)) break;
            lines.Add(line);
        }

        var json = string.Join(Environment.NewLine, lines);
        var dict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json)
                   ?? new Dictionary<string, JsonElement>(StringComparer.OrdinalIgnoreCase);

        var ctor = modelType.GetConstructors()
                       .OrderByDescending(c => c.GetParameters().Length)
                       .FirstOrDefault()
                   ?? throw new InvalidOperationException($"No public constructor found for {modelType.FullName}.");

        var ps = ctor.GetParameters();
        var args = new object?[ps.Length];

        for (var i = 0; i < ps.Length; i++)
        {
            var p = ps[i];
            var name = p.Name!;

            if (!dict.TryGetValue(name, out var el))
            {
                args[i] = ConvertInputSafe("", p.ParameterType, name);
                continue;
            }

            var raw = el.ValueKind switch
            {
                JsonValueKind.String => el.GetString() ?? "",
                _ => el.ToString()
            };

            args[i] = ConvertInputSafe(raw, p.ParameterType, name);
        }

        return ctor.Invoke(args);
    }

    private static string GenerateSampleValue(Type type, string fieldName)
    {
        var t = Nullable.GetUnderlyingType(type) ?? type;

        if (t == typeof(string))
        {
            if (fieldName.Contains("email", StringComparison.OrdinalIgnoreCase)) return "tester@tawtheef.local";
            if (fieldName.Contains("name", StringComparison.OrdinalIgnoreCase)) return "Sample Name";
            if (fieldName.Contains("code", StringComparison.OrdinalIgnoreCase)) return "SAMPLE-001";
            if (fieldName.Contains("url", StringComparison.OrdinalIgnoreCase)) return "https://localhost";
            return "Sample";
        }

        if (t == typeof(Guid)) return Guid.NewGuid().ToString();
        if (t == typeof(DateTime)) return DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture);
        if (t == typeof(DateOnly)) return DateOnly.FromDateTime(DateTime.UtcNow).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        if (t == typeof(TimeOnly)) return TimeOnly.FromDateTime(DateTime.UtcNow).ToString("HH:mm:ss", CultureInfo.InvariantCulture);
        if (t == typeof(int)) return "1";
        if (t == typeof(long)) return "1";
        if (t == typeof(decimal)) return "10.0";
        if (t == typeof(bool)) return "true";

        if (t.IsEnum)
        {
            var values = Enum.GetNames(t);
            return values.Length > 0 ? values[0] : "0";
        }

        return "";
    }

    private static object? ConvertInputSafe(string value, Type targetType, string fieldName)
    {
        var nonNullable = Nullable.GetUnderlyingType(targetType) ?? targetType;

        if (string.IsNullOrWhiteSpace(value))
        {
            if (nonNullable.IsValueType) return Activator.CreateInstance(nonNullable);
            return null;
        }

        try
        {
            if (nonNullable == typeof(string)) return value;
            if (nonNullable == typeof(Guid)) return Guid.Parse(value);
            if (nonNullable == typeof(DateTime)) return DateTime.Parse(value, CultureInfo.InvariantCulture);
            if (nonNullable == typeof(DateOnly)) return DateOnly.Parse(value, CultureInfo.InvariantCulture);
            if (nonNullable == typeof(TimeOnly)) return TimeOnly.Parse(value, CultureInfo.InvariantCulture);
            if (nonNullable.IsEnum) return Enum.Parse(nonNullable, value, ignoreCase: true);
            return Convert.ChangeType(value, nonNullable, CultureInfo.InvariantCulture);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Invalid value for '{fieldName}' ({nonNullable.Name}). Using sample default. Error: {ex.Message}");
            return ConvertInputSafe(GenerateSampleValue(targetType, fieldName), targetType, fieldName);
        }
    }
}
