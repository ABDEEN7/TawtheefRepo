namespace Seeds.ConsoleUi;

internal static class ConsoleUi
{
    public static void PrintHeader(string title)
    {
        System.Console.WriteLine(new string('=', 72));
        System.Console.WriteLine($"  {title}");
        System.Console.WriteLine(new string('=', 72));
        System.Console.WriteLine();
    }

    public static void Footer()
    {
        System.Console.WriteLine();
        System.Console.WriteLine(new string('-', 72));
        System.Console.WriteLine("Press any key to exit...");
        System.Console.ReadKey(intercept: true);
    }

    public static void PrintSection(string title)
    {
        System.Console.WriteLine();
        System.Console.WriteLine($"--- {title} ---");
    }

    public static void Ok(string message) => WriteColored(message, ConsoleColor.Green);
    public static void Warn(string message) => WriteColored(message, ConsoleColor.Yellow);
    public static void Error(string message) => WriteColored(message, ConsoleColor.Red);

    public static void StepStart(string title) => System.Console.Write($"[....] {title} ");

    public static void StepOk(string title)
    {
        System.Console.WriteLine();
        WriteColored($"[ OK ] {title}", ConsoleColor.Green);
    }

    public static void StepFail(string title, string reason)
    {
        System.Console.WriteLine();
        WriteColored($"[FAIL] {title} :: {reason}", ConsoleColor.Red);
    }

    public static string PromptConnectionString(IReadOnlyList<SavedConnection> presets)
    {
        PrintSection("Connection");
        System.Console.WriteLine("Choose a connection string:");
        for (var i = 0; i < presets.Count; i++)
            System.Console.WriteLine($"  {i + 1}) {presets[i].Name}  ({MaskConnection(presets[i].ConnectionString)})");
        System.Console.WriteLine($"  {presets.Count + 1}) Enter a new connection string");
        System.Console.WriteLine();

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
            System.Console.WriteLine($"  {i + 1,2}) {render(items[i])}");

        if (allowAllKeyword)
        {
            System.Console.WriteLine();
            System.Console.WriteLine("Type: 1,3,5  or 2-6  or all");
        }

        System.Console.WriteLine();

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

            return indices
                .OrderBy(i => i)
                .Select(i => items[i - 1])
                .ToList();
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
            System.Console.Write(message);
            var raw = System.Console.ReadLine();
            if (int.TryParse(raw, out var n) && n >= min && n <= max)
                return n;

            Warn($"Enter a number between {min} and {max}.");
        }
    }

    private static string PromptText(string message)
    {
        System.Console.Write(message);
        return System.Console.ReadLine() ?? string.Empty;
    }

    private static bool IsLikelyConnectionString(string cs)
        => cs.Contains("Server=", StringComparison.OrdinalIgnoreCase) ||
           cs.Contains("Data Source=", StringComparison.OrdinalIgnoreCase);

    private static void WriteColored(string text, ConsoleColor color)
    {
        var old = System.Console.ForegroundColor;
        System.Console.ForegroundColor = color;
        System.Console.WriteLine(text);
        System.Console.ForegroundColor = old;
    }
}

internal sealed record SavedConnection(string Name, string ConnectionString);
