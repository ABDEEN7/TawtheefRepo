using System.Net;
using System.Text.RegularExpressions;

namespace Application.Operation.Features.Employee.QuestionBankAssignments.Services;

/// <summary>Conservative policy for the HTML emitted by the question-bank Quill toolbar.</summary>
internal static partial class RichContentPolicy
{
    private static readonly HashSet<string> AllowedTags = new(StringComparer.OrdinalIgnoreCase)
    { "p", "br", "strong", "b", "em", "i", "u", "sub", "sup", "ol", "ul", "li", "span" };

    [GeneratedRegex(@"<\s*(/?)\s*([a-zA-Z0-9]+)([^>]*)>", RegexOptions.Compiled)]
    private static partial Regex TagRegex();
    [GeneratedRegex(@"<(script|iframe|object|embed|style)[^>]*>[\s\S]*?</\1\s*>", RegexOptions.IgnoreCase)]
    private static partial Regex DangerousBlockRegex();
    [GeneratedRegex("""class\s*=\s*(['"])(?<value>[^'"]*)\1""", RegexOptions.IgnoreCase)]
    private static partial Regex ClassRegex();
    [GeneratedRegex("""data-value\s*=\s*(['"])(?<value>[^'"]*)\1""", RegexOptions.IgnoreCase)]
    private static partial Regex FormulaRegex();
    [GeneratedRegex(@"&nbsp;|\u00a0|\s+", RegexOptions.IgnoreCase)]
    private static partial Regex SpaceRegex();

    public static bool HasMeaningfulContent(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return false;
        if (value.Contains("ql-formula", StringComparison.OrdinalIgnoreCase) &&
            FormulaRegex().Match(value).Groups["value"].Value.Trim().Length > 0) return true;
        var text = WebUtility.HtmlDecode(TagRegex().Replace(value, string.Empty));
        return SpaceRegex().Replace(text, string.Empty).Length > 0;
    }

    public static string? Sanitize(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        var withoutBlocks = DangerousBlockRegex().Replace(value, string.Empty);
        return TagRegex().Replace(withoutBlocks, match =>
        {
            var closing = match.Groups[1].Value.Length > 0;
            var tag = match.Groups[2].Value.ToLowerInvariant();
            if (!AllowedTags.Contains(tag)) return string.Empty;
            if (closing) return tag == "br" ? string.Empty : $"</{tag}>";
            if (tag != "span") return tag == "br" ? "<br>" : $"<{tag}>";
            var classes = ClassRegex().Match(match.Value).Groups["value"].Value
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Where(x => x is "ql-formula" or "ql-align-center" or "ql-align-right" or "ql-align-justify")
                .ToArray();
            var formula = FormulaRegex().Match(match.Value).Groups["value"].Value;
            var attributes = classes.Length == 0 ? "" : $" class=\"{string.Join(' ', classes)}\"";
            if (classes.Contains("ql-formula") && formula.Length > 0)
                attributes += $" data-value=\"{WebUtility.HtmlEncode(WebUtility.HtmlDecode(formula))}\"";
            return $"<span{attributes}>";
        }).Trim();
    }
}
