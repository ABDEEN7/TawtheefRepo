using System.Net;
using System.Text;

namespace Application.Operation.Features.Employee.QuestionBankAssignments.Services;

/// <summary>
/// Allowlist sanitizer for the HTML produced by the question-authoring Quill toolbar.
/// Parsing is deliberately performed by a tokenizer rather than by regular expressions.
/// </summary>
public sealed class RichTextSanitizer : IRichTextSanitizer
{
    private static readonly HashSet<string> AllowedTags = new(StringComparer.OrdinalIgnoreCase)
    {
        "p", "br", "strong", "b", "em", "i", "u", "sub", "sup", "ol", "ul", "li", "span"
    };

    private static readonly HashSet<string> AllowedClasses = new(StringComparer.Ordinal)
    {
        "ql-align-center", "ql-align-right", "ql-align-justify", "ql-direction-rtl", "ql-formula", "ql-ui"
    };

    public string? Sanitize(string? html)
    {
        if (string.IsNullOrWhiteSpace(html)) return null;

        var output = new StringBuilder(html.Length);
        var position = 0;
        while (position < html.Length)
        {
            var tagStart = html.IndexOf('<', position);
            if (tagStart < 0)
            {
                AppendText(output, html[position..]);
                break;
            }

            AppendText(output, html[position..tagStart]);
            if (!TryReadTag(html, tagStart, out var tagEnd, out var tag))
            {
                output.Append("&lt;");
                position = tagStart + 1;
                continue;
            }

            AppendAllowedTag(output, tag);
            position = tagEnd + 1;
        }

        var sanitized = output.ToString().Trim();
        return sanitized.Length == 0 ? null : sanitized;
    }

    public bool HasMeaningfulContent(string? html)
    {
        var sanitized = Sanitize(html);
        if (sanitized is null) return false;

        var position = 0;
        var text = new StringBuilder();
        while (position < sanitized.Length)
        {
            var tagStart = sanitized.IndexOf('<', position);
            if (tagStart < 0)
            {
                text.Append(sanitized[position..]);
                break;
            }

            text.Append(sanitized[position..tagStart]);
            if (!TryReadTag(sanitized, tagStart, out var tagEnd, out var tag))
            {
                text.Append('<');
                position = tagStart + 1;
                continue;
            }

            if (!tag.Closing && tag.Name.Equals("span", StringComparison.OrdinalIgnoreCase) &&
                ParseAttributes(tag.Attributes).TryGetValue("class", out var classes) &&
                classes.Split(' ', StringSplitOptions.RemoveEmptyEntries).Contains("ql-formula") &&
                ParseAttributes(tag.Attributes).TryGetValue("data-value", out var formula) &&
                !string.IsNullOrWhiteSpace(WebUtility.HtmlDecode(formula)))
            {
                return true;
            }

            position = tagEnd + 1;
        }

        return WebUtility.HtmlDecode(text.ToString()).Any(character =>
            !char.IsWhiteSpace(character) && character != '\u00a0' && character != '\u200b' && character != '\ufeff');
    }

    private static void AppendAllowedTag(StringBuilder output, ParsedTag tag)
    {
        if (!AllowedTags.Contains(tag.Name)) return;
        var name = tag.Name.ToLowerInvariant();
        if (tag.Closing)
        {
            if (name != "br") output.Append("</").Append(name).Append('>');
            return;
        }

        output.Append('<').Append(name);
        var attributes = ParseAttributes(tag.Attributes);
        var allowedClasses = attributes.GetValueOrDefault("class")?
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Where(cssClass => AllowedClasses.Contains(cssClass) && IsClassAllowedOnTag(cssClass, name))
            .Distinct(StringComparer.Ordinal)
            .ToArray() ?? [];

        if (allowedClasses.Length > 0)
            AppendAttribute(output, "class", string.Join(' ', allowedClasses));

        if (name == "span" && allowedClasses.Contains("ql-formula") &&
            attributes.TryGetValue("data-value", out var formula) && !string.IsNullOrWhiteSpace(formula))
            AppendAttribute(output, "data-value", WebUtility.HtmlDecode(formula));

        if (name == "span" && allowedClasses.Contains("ql-ui"))
            AppendAttribute(output, "contenteditable", "false");

        if (name == "li" && attributes.TryGetValue("data-list", out var listType) &&
            listType is "ordered" or "bullet")
            AppendAttribute(output, "data-list", listType);

        if ((name is "p" or "li") && attributes.TryGetValue("dir", out var direction) && direction == "rtl")
            AppendAttribute(output, "dir", direction);

        output.Append('>');
    }

    private static bool IsClassAllowedOnTag(string cssClass, string tag) => cssClass switch
    {
        "ql-formula" or "ql-ui" => tag == "span",
        "ql-align-center" or "ql-align-right" or "ql-align-justify" or "ql-direction-rtl" =>
            tag is "p" or "li",
        _ => false
    };

    private static void AppendText(StringBuilder output, string text)
        => output.Append(WebUtility.HtmlEncode(WebUtility.HtmlDecode(text)));

    private static void AppendAttribute(StringBuilder output, string name, string value)
        => output.Append(' ').Append(name).Append("=\"").Append(WebUtility.HtmlEncode(value)).Append('"');

    private static bool TryReadTag(string html, int start, out int end, out ParsedTag tag)
    {
        end = -1;
        tag = default;
        var quote = '\0';
        for (var index = start + 1; index < html.Length; index++)
        {
            var character = html[index];
            if (quote != '\0')
            {
                if (character == quote) quote = '\0';
                continue;
            }

            if (character is '\'' or '"') { quote = character; continue; }
            if (character != '>') continue;

            end = index;
            var content = html[(start + 1)..index].Trim();
            if (content.Length == 0 || content.StartsWith('!') || content.StartsWith('?')) return true;
            var closing = content.StartsWith('/');
            if (closing) content = content[1..].TrimStart();
            var nameLength = 0;
            while (nameLength < content.Length && char.IsAsciiLetterOrDigit(content[nameLength])) nameLength++;
            if (nameLength == 0) return true;
            tag = new ParsedTag(content[..nameLength], content[nameLength..], closing);
            return true;
        }

        return false;
    }

    private static Dictionary<string, string> ParseAttributes(string source)
    {
        var attributes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var position = 0;
        while (position < source.Length)
        {
            while (position < source.Length && (char.IsWhiteSpace(source[position]) || source[position] == '/')) position++;
            var nameStart = position;
            while (position < source.Length &&
                   (char.IsAsciiLetterOrDigit(source[position]) || source[position] is '-' or '_')) position++;
            if (position == nameStart) { position++; continue; }
            var name = source[nameStart..position];
            while (position < source.Length && char.IsWhiteSpace(source[position])) position++;
            if (position >= source.Length || source[position] != '=') continue;
            position++;
            while (position < source.Length && char.IsWhiteSpace(source[position])) position++;
            if (position >= source.Length) break;
            var quote = source[position] is '\'' or '"' ? source[position++] : '\0';
            var valueStart = position;
            if (quote == '\0')
                while (position < source.Length && !char.IsWhiteSpace(source[position]) && source[position] != '>') position++;
            else
                while (position < source.Length && source[position] != quote) position++;
            attributes.TryAdd(name, source[valueStart..position]);
            if (quote != '\0' && position < source.Length) position++;
        }
        return attributes;
    }

    private readonly record struct ParsedTag(string Name, string Attributes, bool Closing);
}
