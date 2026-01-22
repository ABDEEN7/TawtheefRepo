using System.Text;
using System.Text.Json;

namespace Tawtheef.Notifications.TemplateTester;

internal static class StateStore
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
