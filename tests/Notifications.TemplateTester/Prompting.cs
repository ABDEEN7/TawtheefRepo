using System.Net.Mail;

namespace Tawtheef.Notifications.TemplateTester;

internal static class Prompting
{
    public static string Prompt(string label, string? defaultValue = null, bool required = true)
    {
        while (true)
        {
            var suffix = string.IsNullOrWhiteSpace(defaultValue) ? "" : $" [{defaultValue}]";
            Console.Write($"{label}{suffix}: ");
            var input = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(input))
                return input.Trim();

            if (!string.IsNullOrWhiteSpace(defaultValue))
                return defaultValue;

            if (!required)
                return "";

            Console.WriteLine("This field is required.");
        }
    }

    public static List<string> PromptEmailList(string label, List<string>? defaultValue, bool required)
    {
        while (true)
        {
            var defaultText = defaultValue is { Count: > 0 } ? string.Join(", ", defaultValue) : null;
            var input = Prompt(label, defaultText, required: required);

            var values = input
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToList();

            if (values.Count == 0 && !required)
                return new List<string>();

            if (values.Count == 0 && required)
            {
                Console.WriteLine("At least one email is required.");
                continue;
            }

            var invalid = values.Where(e => !MailAddress.TryCreate(e, out _)).ToList();
            if (invalid.Count > 0)
            {
                Console.WriteLine($"Invalid emails: {string.Join(", ", invalid)}");
                continue;
            }

            return values;
        }
    }
}
