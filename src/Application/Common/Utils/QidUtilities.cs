using System.Text;

namespace Tawtheef.Application.Common.Utils;

public static class QidUtilities
{
    public const int ExpectedLength = 11;

    public static string Normalize(string? qid)
    {
        if (string.IsNullOrWhiteSpace(qid)) return string.Empty;

        var builder = new StringBuilder(qid.Length);

        foreach (var ch in qid)
        {
            if (char.IsDigit(ch))
            {
                builder.Append(ch);
            }
        }

        return builder.ToString();
    }

    public static bool IsValid(string? qid)
    {
        var normalized = Normalize(qid);
        return normalized.Length == ExpectedLength;
    }
}
