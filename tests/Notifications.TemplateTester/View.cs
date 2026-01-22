namespace Tawtheef.Notifications.TemplateTester;

internal static class View
{
    public static IEnumerable<TemplateEntry> ApplyView(
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

    public static void PrintHeader(string filter, bool onlyFav, TesterState state)
    {
        Console.WriteLine();
        Console.WriteLine($"Filter : {(string.IsNullOrWhiteSpace(filter) ? "-" : filter)}");
        Console.WriteLine($"FavOnly: {(onlyFav ? "ON" : "OFF")}");
        Console.WriteLine($"Profile: {(state.ActiveProfile ?? "-")}");
        Console.WriteLine();
    }

    public static void PrintTemplates(List<TemplateEntry> visible)
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

    public static void PrintHelp()
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

    public static void PrintRecents(TesterState state)
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
}
