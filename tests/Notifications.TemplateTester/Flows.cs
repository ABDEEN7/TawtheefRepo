using System.Text;
using Tawtheef.Application.Common.Interfaces.NotificationServices;
using Tawtheef.Application.Common.Models.Notification;
using Tawtheef.Notifications.Interfaces;

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

        var language = Prompting.Prompt("Language (ar/en)", "ar", required: true).ToLowerInvariant();
        if (language is not ("ar" or "en")) language = "ar";

        var subjectDefaultValue = language == "ar" 
            ? templateEntry.Attribute.SubjectAr 
            : templateEntry.Attribute.SubjectEn;

        var subjectDefault = $"{(profile?.SubjectPrefix ?? "")}{subjectDefaultValue}";
        var subject = Prompting.Prompt("Subject", subjectDefault, required: true);

        var to = Prompting.PromptEmailList("To (comma-separated)", profile?.To, required: true);
        var cc = Prompting.PromptEmailList("CC (comma-separated)", profile?.Cc, required: false);

        var model = ModelBuilder.BuildModelFast(templateKey, modelType, state);

        Console.WriteLine("Rendering...");
        var html = await renderer.RenderHtmlAsync(templateKey, (dynamic)model, language);
        var text = await renderer.RenderTextAsync(templateKey, (dynamic)model, language);

        Console.WriteLine();
        Console.WriteLine("Summary:");
        Console.WriteLine($"  Language: {language}");
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
            var (htmlPath, txtPath) = SavePreview(templateKey, (string)html, (string)text, language);
             Console.WriteLine($"Preview saved ({language}):");
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

    public static async Task SendBatchFlow(
        List<TemplateEntry> templates,
        TesterState state,
        IEmailTemplateRenderer renderer,
        IEmailTransport transport)
    {
        if (templates.Count == 0)
        {
            Console.WriteLine("No templates in the current view to send.");
            return;
        }

        Console.WriteLine($"Batch Send: {templates.Count} templates");
        var profile = GetActiveProfile(state);

        var language = Prompting.Prompt("Language (ar/en)", "ar", required: true).ToLowerInvariant();
        if (language is not ("ar" or "en")) language = "ar";

        var to = Prompting.PromptEmailList("To (comma-separated)", profile?.To, required: true);
        var cc = Prompting.PromptEmailList("CC (comma-separated)", profile?.Cc, required: false);

        Console.WriteLine();
        Console.WriteLine("Summary:");
        Console.WriteLine($"  Language: {language}");
        Console.WriteLine($"  To      : {string.Join(", ", to)}");
        Console.WriteLine($"  Cc      : {(cc.Count == 0 ? "-" : string.Join(", ", cc))}");
        Console.WriteLine($"  Count   : {templates.Count}");
        Console.WriteLine();

        var confirm = Prompting.Prompt($"Send all {templates.Count} templates now? (y/n)", "n", required: true).ToLowerInvariant();
        if (confirm is not ("y" or "yes"))
        {
            Console.WriteLine("Cancelled.");
            return;
        }

        foreach (var t in templates)
        {
            var key = t.Attribute.TemplateKey;
            Console.Write($"Sending {key}... ");
            try
            {
                var model = ModelBuilder.BuildModelFast(key, t.Type, state);
                var html = await renderer.RenderHtmlAsync(key, (dynamic)model, language);
                var text = await renderer.RenderTextAsync(key, (dynamic)model, language);

                var subjectBase = language == "ar" ? t.Attribute.SubjectAr : t.Attribute.SubjectEn;
                var subject = $"{(profile?.SubjectPrefix ?? "")}{subjectBase}";

                var envelope = new EmailEnvelope(to, cc, subject, html, text);
                await transport.SendAsync(envelope);
                Console.WriteLine("OK");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FAILED: {ex.Message}");
            }
        }

        Console.WriteLine();
        Console.WriteLine("Batch send completed.");
    }

    private static (string HtmlPath, string TextPath) SavePreview(string templateKey, string html, string text, string language)
    {
        Directory.CreateDirectory("out");
        var safe = SanitizeFileName(templateKey);
        var ts = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        var htmlPath = Path.Combine("out", $"{safe}_{language}_{ts}.html");
        var txtPath = Path.Combine("out", $"{safe}_{language}_{ts}.txt");

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
