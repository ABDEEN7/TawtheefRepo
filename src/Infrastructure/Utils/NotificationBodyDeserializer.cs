using System.Text.Json;
using System.Text.Json.Serialization;
using Tawtheef.Notifications.Utils;

namespace Tawtheef.Infrastructure.Utils;

public static class NotificationBodyDeserializer
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public static object DeserializeBody(string templateKey, string? body)
    {
        if (string.IsNullOrWhiteSpace(body))
            return new { }; // or throw if body is mandatory for template

        var modelType = NotificationTemplateRegistry.GetModelTypeFor(templateKey);
        var model = JsonSerializer.Deserialize(body, modelType, Options);
        if (model is null)
            throw new InvalidOperationException($"Failed to deserialize body for TemplateKey '{templateKey}' to '{modelType.Name}'.");

        return model;
    }
}
