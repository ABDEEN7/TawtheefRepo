using System.Text;
using Tawtheef.Application.Common.Interfaces.NotificationServices;
using Tawtheef.Application.Common.Models.Notification;
using Tawtheef.Notifications.Interfaces;
using Tawtheef.Notifications.Services;

namespace Tawtheef.Notifications.TemplateTester;

internal static class Flows
{
    public static async Task SendFlow(
        TemplateEntry templateEntry,
        string actionDefault,
        TesterState state,
        IEmailTemplateRenderer renderer,
        IEmailTransport transport)
    {
        var templateKey = templateEntry.Attribute.TemplateKey;
        var modelType = templateEntry.Type;

        TouchRecent(state, templateKey);
        StateStore.Save(state);

        Console.WriteLine();
        Console.WriteLine($"Template: {templateKey}");
        Console.WriteLine(new string('-', Math.Min(60, templateKey.Length + 10)));

        var profile = GetActiveProfile(state);

        var subjectDefault = $"{(profile?.SubjectPrefix ?? "")}{templateKey} - {DateTime.Now:yyyy-MM-dd HH:mm}";
        var subject = Prompting.Prompt("Subject", subjectDefault, required: true);

        var to = Prompting.PromptEmailList("To (comma-separated)", profile?.To, required: true);
        var cc = Prompting.PromptEmailList("CC (comma-separated)", profile?.Cc, required: false);

        var model = ModelBuilder.BuildModelFast(templateKey, modelType, state);

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

        var action = Prompting.Prompt("Action (send/preview/dry)", actionDefault, required: true).ToLowerInvariant();
        if (action is not ("send" or "preview" or "dry"))
            action = actionDefault;

        if (action is "preview" or "dry")
        {
            var (htmlPath, txtPath) = SavePreview(templateKey, (string)html, (string)text);
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

        var confirm = Prompting.Prompt("Confirm send? (y/n)", "n", required: true).ToLowerInvariant();
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

    private static (string HtmlPath, string TextPath) SavePreview(string templateKey, string html, string text)
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

    private static string SanitizeFileName(string name)
    {
        foreach (var ch in Path.GetInvalidFileNameChars())
            name = name.Replace(ch, '_');

        return name.Replace(':', '_').Replace('/', '_').Replace('\\', '_');
    }

    private static void TouchRecent(TesterState state, string templateKey)
    {
        state.Recents.RemoveAll(x => string.Equals(x, templateKey, StringComparison.OrdinalIgnoreCase));
        state.Recents.Insert(0, templateKey);
        if (state.Recents.Count > 10) state.Recents.RemoveRange(10, state.Recents.Count - 10);
    }

    private static EmailProfile? GetActiveProfile(TesterState state)
    {
        if (state.ActiveProfile is null) return null;
        return state.Profiles.TryGetValue(state.ActiveProfile, out var p) ? p : null;
    }
}
