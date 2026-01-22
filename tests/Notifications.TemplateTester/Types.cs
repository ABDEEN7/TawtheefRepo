using Tawtheef.Notifications.Attributes;

namespace Tawtheef.Notifications.TemplateTester;

internal sealed class TesterState
{
    public string? ActiveProfile { get; set; }
    public Dictionary<string, EmailProfile> Profiles { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public HashSet<string> Favorites { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public List<string> Recents { get; set; } = new();
    public Dictionary<string, Dictionary<string, string>> LastModelValuesByTemplate { get; set; }
        = new(StringComparer.OrdinalIgnoreCase);
}

internal sealed class EmailProfile
{
    public List<string> To { get; set; } = new();
    public List<string> Cc { get; set; } = new();
    public string SubjectPrefix { get; set; } = "";
}

internal sealed record TemplateEntry(Type Type, NotificationTemplateAttribute Attribute);
