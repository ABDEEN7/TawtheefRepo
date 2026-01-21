// Program.cs
// Notification Template Tester (UX-enhanced)
// - Search/filter templates
// - Favorites + Recents
// - Profiles (To/Cc/SubjectPrefix)
// - Preview / Dry-run / Send
// - Fast model modes: auto / last / prompt / json
//
// Notes:
// 1) This keeps your existing discovery via NotificationTemplateAttribute.
// 2) CC bug fixed: CC is added to message.CC not To.
// 3) SMTP Credentials remain unset to match your original. Enable if needed.

using System.Globalization;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Tawtheef.Application.Common.Interfaces.NotificationServices;
using Tawtheef.Application.Common.Models.Notification;
using Tawtheef.Domain.Configurations.Settings;
using Tawtheef.Notifications;
using Tawtheef.Notifications.Attributes;
using Tawtheef.Notifications.Context;
using Tawtheef.Notifications.Interfaces;
using Tawtheef.Notifications.Services;
using Tawtheef.Notifications.Utils;

#region Bootstrap (your existing wiring)

NotificationTemplateRegistry.AutoRegisterFrom(typeof(NotificationAssemblyMarker).Assembly);

var templates = typeof(NotificationAssemblyMarker).Assembly
    .GetTypes()
    .Where(type => type is { IsAbstract: false, IsInterface: false })
    .Select(type => new TemplateEntry(type, type.GetCustomAttribute<NotificationTemplateAttribute>()!))
    .Where(entry => entry.Attribute is not null)
    .Select(entry => entry with { Attribute = entry.Attribute! })
    .OrderBy(entry => entry.Attribute.TemplateKey)
    .ToList();

if (templates.Count == 0)
{
    Console.WriteLine("No notification templates found.");
    return;
}

var emailSettings = new EmailSettings
{
    SmtpHost = "smtp.edu.gov.qa",
    SmtpPort = 25,
    EmailUser = "tawtheef@edu.gov.qa",
    EmailPass = "Taw@Theef", // Keep as-is or load from secrets
    ManagerEmails = "manager@tawtheef.local",
    ContactUsEmail = "contact@tawtheef.local",
    ProductName = "Tawtheef"
};

var appConfig = new AppConfigSettings
{
    FrontendUrl = "https://localhost",
    BackendUrl = "https://localhost",
    BlobSignKey = "dev",
    AdminEmail = "admin@tawtheef.local",
    AdminEmails = ["admin@tawtheef.local"],
    DefaultSignedUrlMinutes = 3
};

var branding = new DefaultBranding(Options.Create(emailSettings), Options.Create(appConfig));
var renderer = new RazorTemplateRenderer(branding);
var transport = new FileEmailTransport(emailSettings);

#endregion

#region UX State

var state = StateStore.Load();
var filter = "";
var onlyFav = false;

Console.WriteLine("Notification Template Tester");
Console.WriteLine("============================");
Console.WriteLine("Type 'help' to see commands.");
Console.WriteLine();

MainLoop();

void MainLoop()
{
    while (true)
    {
        var visible = ApplyView(templates, filter, onlyFav, state).ToList();
        PrintHeader(filter, onlyFav, state);
        PrintTemplates(visible);

        Console.WriteLine();
        Console.Write("cmd> ");
        var cmdLine = Console.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(cmdLine))
            continue;

        if (TryHandleCommand(cmdLine, visible))
            continue;
    }
}

bool TryHandleCommand(string cmdLine, List<TemplateEntry> visible)
{
    var parts = SplitCommand(cmdLine);
    var cmd = parts.Count > 0 ? parts[0].ToLowerInvariant() : "";
    var args = parts.Skip(1).ToList();

    switch (cmd)
    {
        case "q":
        case "quit":
        case "exit":
            StateStore.Save(state);
            Environment.Exit(0);
            return true;

        case "help":
            PrintHelp();
            return true;

        case "clear":
            filter = "";
            onlyFav = false;
            return true;

        case "s":
        case "search":
            filter = string.Join(" ", args).Trim();
            return true;

        case "fav":
            HandleFav(args);
            return true;

        case "recents":
            PrintRecents(state);
            return true;

        case "profile":
            HandleProfile(args);
            return true;

        case "show":
            HandleShow(args);
            return true;

        case "send":
            {
                var template = ResolveTemplate(args, visible, templates);
                if (template is null) return true;
                SendFlow(template, actionDefault: "send").GetAwaiter().GetResult();
                return true;
            }

        case "render":
            {
                var template = ResolveTemplate(args, visible, templates);
                if (template is null) return true;
                SendFlow(template, actionDefault: "preview").GetAwaiter().GetResult();
                return true;
            }

        case "dry":
            {
                var template = ResolveTemplate(args, visible, templates);
                if (template is null) return true;
                SendFlow(template, actionDefault: "dry").GetAwaiter().GetResult();
                return true;
            }

        case "all":
            {
                // Render/send all visible templates (defaults to dry-run for safety)
                var action = args.FirstOrDefault()?.ToLowerInvariant() ?? "dry";
                if (action is not ("send" or "preview" or "dry"))
                    action = "dry";

                foreach (var t in visible)
                    SendFlow(t, actionDefault: action).GetAwaiter().GetResult();

                return true;
            }

        default:
            // convenience:
            // - "#3" => send visible index 3
            // - "3"  => send visible index 3
            // - "OfficeCreated" => send template by key
            if (TryResolveIndexOrKey(cmdLine, visible, templates, out var tResolved))
            {
                SendFlow(tResolved!, actionDefault: "send").GetAwaiter().GetResult();
                return true;
            }

            Console.WriteLine("Unknown command. Type 'help'.");
            return true;
    }
}

#endregion

#region Flows

async Task SendFlow(TemplateEntry templateEntry, string actionDefault)
{
    var templateKey = templateEntry.Attribute.TemplateKey;
    var modelType = templateEntry.Type;

    // mark recent
    TouchRecent(state, templateKey);
    StateStore.Save(state);

    Console.WriteLine();
    Console.WriteLine($"Template: {templateKey}");
    Console.WriteLine(new string('-', Math.Min(60, templateKey.Length + 10)));

    var profile = GetActiveProfile(state);

    var subjectDefault = $"{(profile?.SubjectPrefix ?? "")}{templateKey} - {DateTime.Now:yyyy-MM-dd HH:mm}";
    var subject = Prompt("Subject", subjectDefault, required: true);

    var to = PromptEmailList("To (comma-separated)", profile?.To, required: true);
    var cc = PromptEmailList("CC (comma-separated)", profile?.Cc, required: false);

    var model = BuildModelFast(templateKey, modelType, state);

    Console.WriteLine("Rendering...");
    var html = await renderer.RenderHtmlAsync(templateKey, (dynamic)model);
    var text = await renderer.RenderTextAsync(templateKey, (dynamic)model);

    Console.WriteLine();
    Console.WriteLine("Summary:");
    Console.WriteLine($"  To      : {string.Join(", ", to)}");
    Console.WriteLine($"  Cc      : {(cc.Count == 0 ? "-" : string.Join(", ", cc))}");
    Console.WriteLine($"  Subject : {subject}");
    Console.WriteLine($"  Action  : {actionDefault}");
    Console.WriteLine();

    var action = Prompt("Action (send/preview/dry)", actionDefault, required: true).ToLowerInvariant();
    if (action is not ("send" or "preview" or "dry"))
        action = actionDefault;

    if (action is "preview" or "dry")
    {
        var (htmlPath, txtPath) = SavePreview(templateKey, html, text);
        Console.WriteLine($"Preview saved:");
        Console.WriteLine($"  {htmlPath}");
        Console.WriteLine($"  {txtPath}");
        if (action == "dry")
        {
            Console.WriteLine("Dry-run: not sent.");
            Console.WriteLine();
            return;
        }
    }

    // Final confirm before sending
    var confirm = Prompt("Confirm send? (y/n)", "n", required: true).ToLowerInvariant();
    if (confirm is not ("y" or "yes"))
    {
        Console.WriteLine("Cancelled.");
        Console.WriteLine();
        return;
    }

    var envelope = new EmailEnvelope(to, cc, subject, html, text);
    await transport.SendAsync(envelope);

    Console.WriteLine("Sent.");
    Console.WriteLine();
}

static (string HtmlPath, string TextPath) SavePreview(string templateKey, string html, string text)
{
    Directory.CreateDirectory("out");
    var safe = SanitizeFileName(templateKey);
    var ts = DateTime.Now.ToString("yyyyMMdd_HHmmss");
    var htmlPath = Path.Combine("out", $"{safe}_{ts}.html");
    var txtPath = Path.Combine("out", $"{safe}_{ts}.txt");

    File.WriteAllText(htmlPath, html, Encoding.UTF8);
    File.WriteAllText(txtPath, text, Encoding.UTF8);

    return (htmlPath, txtPath);
}

#endregion

#region View / Commands helpers

static IEnumerable<TemplateEntry> ApplyView(
    List<TemplateEntry> all,
    string filter,
    bool onlyFav,
    TesterState state)
{
    IEnumerable<TemplateEntry> q = all;

    if (!string.IsNullOrWhiteSpace(filter))
        q = q.Where(t => t.Attribute.TemplateKey.Contains(filter, StringComparison.OrdinalIgnoreCase));

    if (onlyFav)
        q = q.Where(t => state.Favorites.Contains(t.Attribute.TemplateKey));

    // Favorites first, then recents, then alphabetic (stable)
    var favSet = state.Favorites;
    var recentOrder = state.Recents
        .Select((k, i) => new { k, i })
        .ToDictionary(x => x.k, x => x.i, StringComparer.OrdinalIgnoreCase);

    return q.OrderByDescending(t => favSet.Contains(t.Attribute.TemplateKey))
        .ThenBy(t => recentOrder.TryGetValue(t.Attribute.TemplateKey, out var idx) ? idx : int.MaxValue)
        .ThenBy(t => t.Attribute.TemplateKey);
}

static void PrintHeader(string filter, bool onlyFav, TesterState state)
{
    Console.WriteLine();
    Console.WriteLine($"Filter : {(string.IsNullOrWhiteSpace(filter) ? "-" : filter)}");
    Console.WriteLine($"FavOnly: {(onlyFav ? "ON" : "OFF")}");
    Console.WriteLine($"Profile: {(state.ActiveProfile ?? "-")}");
    Console.WriteLine();
}

static void PrintTemplates(List<TemplateEntry> visible)
{
    Console.WriteLine("Templates:");
    if (visible.Count == 0)
    {
        Console.WriteLine("  (none)");
        return;
    }

    for (var i = 0; i < visible.Count; i++)
        Console.WriteLine($"  {i + 1,3}. {visible[i].Attribute.TemplateKey}");
}

static void PrintHelp()
{
    Console.WriteLine();
    Console.WriteLine("Commands:");
    Console.WriteLine("  help                         Show commands");
    Console.WriteLine("  clear                        Clear filter and favorites-only");
    Console.WriteLine("  search <text> | s <text>      Filter by template key");
    Console.WriteLine("  show fav                      Toggle favorites-only view");
    Console.WriteLine("  show all                      Disable favorites-only view");
    Console.WriteLine("  fav add <key|#n>              Add to favorites");
    Console.WriteLine("  fav rm <key|#n>               Remove from favorites");
    Console.WriteLine("  fav list                      List favorites");
    Console.WriteLine("  recents                       Show recent templates");
    Console.WriteLine();
    Console.WriteLine("  profile list                  List profiles");
    Console.WriteLine("  profile use <name>            Set active profile");
    Console.WriteLine("  profile save <name>           Save current To/Cc/SubjectPrefix as profile (you'll be prompted)");
    Console.WriteLine("  profile rm <name>             Remove profile");
    Console.WriteLine();
    Console.WriteLine("  send <key|#n>                 Render + send one template");
    Console.WriteLine("  render <key|#n>               Render + preview to disk (no send unless you confirm)");
    Console.WriteLine("  dry <key|#n>                  Render + preview, never send");
    Console.WriteLine("  all [send|preview|dry]        Apply to all visible templates (default dry)");
    Console.WriteLine();
    Console.WriteLine("Shortcuts:");
    Console.WriteLine("  3 or #3                       Send visible item #3");
    Console.WriteLine("  OfficeCreatedNotification      Send by template key");
    Console.WriteLine("  q                             Quit");
    Console.WriteLine();
}

void HandleShow(List<string> args)
{
    var mode = args.FirstOrDefault()?.ToLowerInvariant();
    switch (mode)
    {
        case "fav":
        case "favorites":
            onlyFav = !onlyFav;
            Console.WriteLine($"Favorites-only: {(onlyFav ? "ON" : "OFF")}");
            break;

        case "all":
            onlyFav = false;
            Console.WriteLine("Favorites-only: OFF");
            break;

        default:
            Console.WriteLine("Usage: show fav | show all");
            break;
    }
}

void HandleFav(List<string> args)
{
    var sub = args.FirstOrDefault()?.ToLowerInvariant() ?? "";
    var rest = args.Skip(1).ToList();

    switch (sub)
    {
        case "list":
            if (state.Favorites.Count == 0)
            {
                Console.WriteLine("No favorites.");
                return;
            }
            Console.WriteLine("Favorites:");
            foreach (var k in state.Favorites.OrderBy(x => x))
                Console.WriteLine($"  - {k}");
            return;

        case "add":
            {
                var key = rest.Count == 0 ? null : rest[0];
                if (string.IsNullOrWhiteSpace(key))
                {
                    Console.WriteLine("Usage: fav add <key|#n>");
                    return;
                }

                if (!TryMapKeyOrIndex(key, ApplyView(templates, filter, onlyFav, state).ToList(), templates, out var mapped))
                {
                    Console.WriteLine("Template not found.");
                    return;
                }

                state.Favorites.Add(mapped!);
                StateStore.Save(state);
                Console.WriteLine($"Added favorite: {mapped}");
                return;
            }

        case "rm":
        case "remove":
            {
                var key = rest.Count == 0 ? null : rest[0];
                if (string.IsNullOrWhiteSpace(key))
                {
                    Console.WriteLine("Usage: fav rm <key|#n>");
                    return;
                }

                if (!TryMapKeyOrIndex(key, ApplyView(templates, filter, onlyFav, state).ToList(), templates, out var mapped))
                {
                    Console.WriteLine("Template not found.");
                    return;
                }

                state.Favorites.Remove(mapped!);
                StateStore.Save(state);
                Console.WriteLine($"Removed favorite: {mapped}");
                return;
            }

        default:
            Console.WriteLine("Usage: fav add|rm|list ...");
            return;
    }
}

void HandleProfile(List<string> args)
{
    var sub = args.FirstOrDefault()?.ToLowerInvariant() ?? "";
    var rest = args.Skip(1).ToList();

    switch (sub)
    {
        case "list":
            if (state.Profiles.Count == 0)
            {
                Console.WriteLine("No profiles.");
                return;
            }
            Console.WriteLine("Profiles:");
            foreach (var kv in state.Profiles.OrderBy(k => k.Key))
            {
                var p = kv.Value;
                Console.WriteLine($"  - {kv.Key}  | To={p.To.Count} Cc={p.Cc.Count} Prefix='{p.SubjectPrefix}'");
            }
            return;

        case "use":
            {
                var name = rest.FirstOrDefault();
                if (string.IsNullOrWhiteSpace(name))
                {
                    Console.WriteLine("Usage: profile use <name>");
                    return;
                }

                if (!state.Profiles.ContainsKey(name))
                {
                    Console.WriteLine("Profile not found.");
                    return;
                }

                state.ActiveProfile = name;
                StateStore.Save(state);
                Console.WriteLine($"Active profile: {name}");
                return;
            }

        case "save":
            {
                var name = rest.FirstOrDefault();
                if (string.IsNullOrWhiteSpace(name))
                {
                    Console.WriteLine("Usage: profile save <name>");
                    return;
                }

                var to = PromptEmailList("Profile To (comma-separated)", null, required: true);
                var cc = PromptEmailList("Profile Cc (comma-separated)", null, required: false);
                var prefix = Prompt("Subject prefix", "", required: false);

                state.Profiles[name] = new EmailProfile
                {
                    To = to,
                    Cc = cc,
                    SubjectPrefix = prefix ?? ""
                };
                state.ActiveProfile = name;
                StateStore.Save(state);
                Console.WriteLine($"Saved profile: {name}");
                return;
            }

        case "rm":
        case "remove":
            {
                var name = rest.FirstOrDefault();
                if (string.IsNullOrWhiteSpace(name))
                {
                    Console.WriteLine("Usage: profile rm <name>");
                    return;
                }

                if (!state.Profiles.Remove(name))
                {
                    Console.WriteLine("Profile not found.");
                    return;
                }

                if (string.Equals(state.ActiveProfile, name, StringComparison.OrdinalIgnoreCase))
                    state.ActiveProfile = null;

                StateStore.Save(state);
                Console.WriteLine($"Removed profile: {name}");
                return;
            }

        default:
            Console.WriteLine("Usage: profile list|use|save|rm ...");
            return;
    }
}

static void PrintRecents(TesterState state)
{
    if (state.Recents.Count == 0)
    {
        Console.WriteLine("No recents.");
        return;
    }

    Console.WriteLine("Recents:");
    for (var i = 0; i < state.Recents.Count; i++)
        Console.WriteLine($"  {i + 1,2}. {state.Recents[i]}");
}

static void TouchRecent(TesterState state, string templateKey)
{
    state.Recents.RemoveAll(x => string.Equals(x, templateKey, StringComparison.OrdinalIgnoreCase));
    state.Recents.Insert(0, templateKey);
    if (state.Recents.Count > 10) state.Recents.RemoveRange(10, state.Recents.Count - 10);
}

static EmailProfile? GetActiveProfile(TesterState state)
{
    if (state.ActiveProfile is null) return null;
    return state.Profiles.TryGetValue(state.ActiveProfile, out var p) ? p : null;
}

static TemplateEntry? ResolveTemplate(List<string> args, List<TemplateEntry> visible, List<TemplateEntry> all)
{
    if (args.Count == 0)
    {
        Console.WriteLine("Specify a template by key or index (#n). Example: send #2  OR  send OfficeCreatedNotification");
        return null;
    }

    var token = args[0];
    if (!TryMapKeyOrIndex(token, visible, all, out var key))
    {
        Console.WriteLine("Template not found.");
        return null;
    }

    return all.First(t => string.Equals(t.Attribute.TemplateKey, key, StringComparison.OrdinalIgnoreCase));
}

static bool TryResolveIndexOrKey(string raw, List<TemplateEntry> visible, List<TemplateEntry> all, out TemplateEntry? template)
{
    template = null;
    if (!TryMapKeyOrIndex(raw, visible, all, out var key))
        return false;

    template = all.First(t => string.Equals(t.Attribute.TemplateKey, key, StringComparison.OrdinalIgnoreCase));
    return true;
}

static bool TryMapKeyOrIndex(string token, List<TemplateEntry> visible, List<TemplateEntry> all, out string? templateKey)
{
    templateKey = null;
    token = token.Trim();

    if (token.StartsWith("#"))
        token = token[1..];

    if (int.TryParse(token, out var idx))
    {
        if (idx < 1 || idx > visible.Count) return false;
        templateKey = visible[idx - 1].Attribute.TemplateKey;
        return true;
    }

    // Exact match by key
    var exact = all.FirstOrDefault(t => string.Equals(t.Attribute.TemplateKey, token, StringComparison.OrdinalIgnoreCase));
    if (exact is not null)
    {
        templateKey = exact.Attribute.TemplateKey;
        return true;
    }

    // Partial match (single) by contains
    var matches = all.Where(t => t.Attribute.TemplateKey.Contains(token, StringComparison.OrdinalIgnoreCase)).ToList();
    if (matches.Count == 1)
    {
        templateKey = matches[0].Attribute.TemplateKey;
        return true;
    }

    if (matches.Count > 1)
    {
        Console.WriteLine("Ambiguous template key. Matches:");
        foreach (var m in matches.Take(10))
            Console.WriteLine($"  - {m.Attribute.TemplateKey}");
        if (matches.Count > 10) Console.WriteLine($"  ... and {matches.Count - 10} more");
    }

    return false;
}

static List<string> SplitCommand(string input)
{
    // supports quoted args: profile save "QA Team"
    var result = new List<string>();
    var sb = new StringBuilder();
    var inQuotes = false;

    foreach (var c in input)
    {
        if (c == '"')
        {
            inQuotes = !inQuotes;
            continue;
        }

        if (!inQuotes && char.IsWhiteSpace(c))
        {
            if (sb.Length > 0)
            {
                result.Add(sb.ToString());
                sb.Clear();
            }
            continue;
        }

        sb.Append(c);
    }

    if (sb.Length > 0)
        result.Add(sb.ToString());

    return result;
}

static string SanitizeFileName(string name)
{
    foreach (var ch in Path.GetInvalidFileNameChars())
        name = name.Replace(ch, '_');

    return name.Replace(':', '_').Replace('/', '_').Replace('\\', '_');
}

#endregion

#region Prompting + Validation

static string Prompt(string label, string? defaultValue = null, bool required = true)
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

static List<string> PromptEmailList(string label, List<string>? defaultValue, bool required)
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

#endregion

#region Model build (auto/last/prompt/json)

static object BuildModelFast(string templateKey, Type modelType, TesterState state)
{
    Console.WriteLine();
    Console.WriteLine("Model mode: auto | last | prompt | json");
    var mode = Prompt("Choose model mode", "auto", required: true).ToLowerInvariant();

    return mode switch
    {
        "auto" => BuildModelAuto(templateKey, modelType, state),
        "last" => BuildModelFromLast(templateKey, modelType, state),
        "prompt" => BuildModelPrompted(templateKey, modelType, state),
        "json" => BuildModelFromJson(modelType),
        _ => BuildModelAuto(templateKey, modelType, state)
    };
}

static object BuildModelFromLast(string templateKey, Type modelType, TesterState state)
{
    if (!state.LastModelValuesByTemplate.TryGetValue(templateKey, out var last) || last.Count == 0)
    {
        Console.WriteLine("No saved values for this template. Falling back to auto.");
        return BuildModelAuto(templateKey, modelType, state);
    }

    return BuildModelFromDictionary(modelType, last, templateKey, state);
}

static object BuildModelAuto(string templateKey, Type modelType, TesterState state)
{
    var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    var ctor = modelType.GetConstructors()
        .OrderByDescending(c => c.GetParameters().Length)
        .FirstOrDefault() ?? throw new InvalidOperationException($"No public constructor found for {modelType.FullName}.");

    foreach (var p in ctor.GetParameters())
        dict[p.Name!] = GenerateSampleValue(p.ParameterType, p.Name!);

    return BuildModelFromDictionary(modelType, dict, templateKey, state);
}

static object BuildModelPrompted(string templateKey, Type modelType, TesterState state)
{
    var ctor = modelType.GetConstructors()
        .OrderByDescending(c => c.GetParameters().Length)
        .FirstOrDefault() ?? throw new InvalidOperationException($"No public constructor found for {modelType.FullName}.");

    state.LastModelValuesByTemplate.TryGetValue(templateKey, out var last);
    last ??= new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    foreach (var p in ctor.GetParameters())
    {
        var name = p.Name!;
        var typeName = Nullable.GetUnderlyingType(p.ParameterType)?.Name ?? p.ParameterType.Name;

        last.TryGetValue(name, out var lastVal);
        var input = Prompt($"{name} ({typeName})", lastVal, required: false);

        if (string.IsNullOrWhiteSpace(input))
            input = GenerateSampleValue(p.ParameterType, name);

        dict[name] = input;
    }

    return BuildModelFromDictionary(modelType, dict, templateKey, state);
}

static object BuildModelFromDictionary(
    Type modelType,
    Dictionary<string, string> dict,
    string templateKey,
    TesterState state)
{
    var ctor = modelType.GetConstructors()
        .OrderByDescending(c => c.GetParameters().Length)
        .FirstOrDefault() ?? throw new InvalidOperationException($"No public constructor found for {modelType.FullName}.");

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

    // Persist last values per template
    state.LastModelValuesByTemplate[templateKey] = new Dictionary<string, string>(dict, StringComparer.OrdinalIgnoreCase);
    StateStore.Save(state);

    return ctor.Invoke(args);
}

static object BuildModelFromJson(Type modelType)
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
        .FirstOrDefault() ?? throw new InvalidOperationException($"No public constructor found for {modelType.FullName}.");

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

static string GenerateSampleValue(Type type, string fieldName)
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

static object? ConvertInputSafe(string value, Type targetType, string fieldName)
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

#endregion

#region Persistence

sealed class TesterState
{
    public string? ActiveProfile { get; set; }
    public Dictionary<string, EmailProfile> Profiles { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public HashSet<string> Favorites { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public List<string> Recents { get; set; } = new();
    public Dictionary<string, Dictionary<string, string>> LastModelValuesByTemplate { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}

sealed class EmailProfile
{
    public List<string> To { get; set; } = new();
    public List<string> Cc { get; set; } = new();
    public string SubjectPrefix { get; set; } = "";
}

static class StateStore
{
    private const string FileName = "notification-tester.state.json";

    public static TesterState Load()
    {
        if (!File.Exists(FileName)) return new TesterState();
        try
        {
            var json = File.ReadAllText(FileName, Encoding.UTF8);
            return JsonSerializer.Deserialize<TesterState>(json) ?? new TesterState();
        }
        catch
        {
            return new TesterState();
        }
    }

    public static void Save(TesterState state)
    {
        var json = JsonSerializer.Serialize(state, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(FileName, json, Encoding.UTF8);
    }
}

#endregion

#region Transport + TemplateEntry

sealed class FileEmailTransport(EmailSettings emailSettings) : IEmailTransport
{
    public Task SendAsync(EmailEnvelope envelope, CancellationToken ct = default)
    {
        using var message = new MailMessage
        {
            From = new MailAddress(emailSettings.EmailUser),
            Subject = envelope.Subject,
            Body = envelope.HtmlBody,
            IsBodyHtml = true,
        };

        foreach (var to in envelope.To)
            message.To.Add(to);

        foreach (var cc in envelope.Cc ?? [])
            message.CC.Add(cc); // FIX: CC goes to CC collection

        using var smtp = new SmtpClient(emailSettings.SmtpHost, emailSettings.SmtpPort)
        {
            EnableSsl = true,
            // IMPORTANT: no Credentials set (same as your original)
            // If required:
            // Credentials = new NetworkCredential(emailSettings.EmailUser, emailSettings.EmailPass)
        };

        smtp.Send(message);
        return Task.CompletedTask;
    }
}

file sealed record TemplateEntry(Type Type, NotificationTemplateAttribute Attribute);

#endregion
