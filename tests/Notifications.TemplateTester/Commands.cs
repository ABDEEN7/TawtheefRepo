using System.Text;

namespace Tawtheef.Notifications.TemplateTester;

internal static class Commands
{
    public static bool TryHandleCommand(
        string cmdLine,
        List<TemplateEntry> visible,
        List<TemplateEntry> templates,
        TesterState state,
        dynamic renderer,
        dynamic transport,
        ref string filter,
        ref bool onlyFav)
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
                View.PrintHelp();
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
                HandleFav(args, templates, state, filter, onlyFav);
                return true;

            case "recents":
                View.PrintRecents(state);
                return true;

            case "profile":
                HandleProfile(args, state);
                return true;

            case "show":
                HandleShow(args, ref onlyFav);
                return true;

            case "send":
                {
                    var template = ResolveTemplate(args, visible, templates);
                    if (template is null) return true;
                    Flows.SendFlow(template, "send", state, renderer, transport).GetAwaiter().GetResult();
                    return true;
                }

            case "render":
                {
                    var template = ResolveTemplate(args, visible, templates);
                    if (template is null) return true;
                    Flows.SendFlow(template, "preview", state, renderer, transport).GetAwaiter().GetResult();
                    return true;
                }

            case "dry":
                {
                    var template = ResolveTemplate(args, visible, templates);
                    if (template is null) return true;
                    Flows.SendFlow(template, "dry", state, renderer, transport).GetAwaiter().GetResult();
                    return true;
                }

            case "all":
                {
                    var action = args.FirstOrDefault()?.ToLowerInvariant() ?? "dry";
                    if (action is not ("send" or "preview" or "dry"))
                        action = "dry";

                    foreach (var t in visible)
                        Flows.SendFlow(t, action, state, renderer, transport).GetAwaiter().GetResult();

                    return true;
                }

            default:
                if (TryResolveIndexOrKey(cmdLine, visible, templates, out var tResolved))
                {
                    Flows.SendFlow(tResolved!, "send", state, renderer, transport).GetAwaiter().GetResult();
                    return true;
                }

                Console.WriteLine("Unknown command. Type 'help'.");
                return true;
        }
    }

    private static void HandleShow(List<string> args, ref bool onlyFav)
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

    private static void HandleFav(
        List<string> args,
        List<TemplateEntry> templates,
        TesterState state,
        string filter,
        bool onlyFav)
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

                    var currentVisible = View.ApplyView(templates, filter, onlyFav, state).ToList();
                    if (!TryMapKeyOrIndex(key, currentVisible, templates, out var mapped))
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

                    var currentVisible = View.ApplyView(templates, filter, onlyFav, state).ToList();
                    if (!TryMapKeyOrIndex(key, currentVisible, templates, out var mapped))
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

    private static void HandleProfile(List<string> args, TesterState state)
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

                    var to = Prompting.PromptEmailList("Profile To (comma-separated)", null, required: true);
                    var cc = Prompting.PromptEmailList("Profile Cc (comma-separated)", null, required: false);
                    var prefix = Prompting.Prompt("Subject prefix", "", required: false);

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

    private static EmailProfile? GetActiveProfile(TesterState state)
    {
        if (state.ActiveProfile is null) return null;
        return state.Profiles.TryGetValue(state.ActiveProfile, out var p) ? p : null;
    }

    private static TemplateEntry? ResolveTemplate(List<string> args, List<TemplateEntry> visible, List<TemplateEntry> all)
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

    private static bool TryResolveIndexOrKey(string raw, List<TemplateEntry> visible, List<TemplateEntry> all, out TemplateEntry? template)
    {
        template = null;
        if (!TryMapKeyOrIndex(raw, visible, all, out var key))
            return false;

        template = all.First(t => string.Equals(t.Attribute.TemplateKey, key, StringComparison.OrdinalIgnoreCase));
        return true;
    }

    private static bool TryMapKeyOrIndex(string token, List<TemplateEntry> visible, List<TemplateEntry> all, out string? templateKey)
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

        var exact = all.FirstOrDefault(t => string.Equals(t.Attribute.TemplateKey, token, StringComparison.OrdinalIgnoreCase));
        if (exact is not null)
        {
            templateKey = exact.Attribute.TemplateKey;
            return true;
        }

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

    private static List<string> SplitCommand(string input)
    {
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
}
