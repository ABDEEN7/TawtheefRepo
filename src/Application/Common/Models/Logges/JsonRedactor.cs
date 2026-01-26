using System.Text;
using System.Text.Json;

namespace Tawtheef.Application.Common.Models.Logges;

public static class JsonRedactor
{
    public static string RedactJson(string json, IReadOnlyCollection<string> sensitiveKeys)
    {
        if (string.IsNullOrWhiteSpace(json)) return string.Empty;

        using var doc = JsonDocument.Parse(json);

        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = false }))
        {
            WriteElement(doc.RootElement, writer, sensitiveKeys);
        }

        return Encoding.UTF8.GetString(stream.ToArray());
    }

    private static void WriteElement(JsonElement element, Utf8JsonWriter writer, IReadOnlyCollection<string> sensitiveKeys)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                writer.WriteStartObject();
                foreach (var prop in element.EnumerateObject())
                {
                    writer.WritePropertyName(prop.Name);

                    if (IsSensitive(prop.Name, sensitiveKeys))
                    {
                        writer.WriteStringValue("***");
                        continue;
                    }

                    WriteElement(prop.Value, writer, sensitiveKeys);
                }
                writer.WriteEndObject();
                break;

            case JsonValueKind.Array:
                writer.WriteStartArray();
                foreach (var item in element.EnumerateArray())
                    WriteElement(item, writer, sensitiveKeys);
                writer.WriteEndArray();
                break;

            case JsonValueKind.String:
                writer.WriteStringValue(element.GetString());
                break;

            case JsonValueKind.Number:
                if (element.TryGetInt64(out var l)) writer.WriteNumberValue(l);
                else if (element.TryGetDouble(out var d)) writer.WriteNumberValue(d);
                else writer.WriteRawValue(element.GetRawText());
                break;

            case JsonValueKind.True:
            case JsonValueKind.False:
                writer.WriteBooleanValue(element.GetBoolean());
                break;

            case JsonValueKind.Null:
            case JsonValueKind.Undefined:
                writer.WriteNullValue();
                break;

            default:
                writer.WriteRawValue(element.GetRawText());
                break;
        }
    }

    private static bool IsSensitive(string key, IReadOnlyCollection<string> sensitiveKeys)
    {
        // case-insensitive match; also supports "Authorization" etc.
        foreach (var s in sensitiveKeys)
        {
            if (key.Equals(s, StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }
}
