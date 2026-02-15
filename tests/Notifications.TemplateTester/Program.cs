using Tawtheef.Notifications;
using Tawtheef.Notifications.TemplateTester;
using Tawtheef.Notifications.Utils;

NotificationTemplateRegistry.AutoRegisterFrom(typeof(NotificationAssemblyMarker).Assembly);

var templates = Bootstrap.LoadTemplates(typeof(NotificationAssemblyMarker).Assembly);
if (templates.Count == 0)
{
    Console.WriteLine("No notification templates found.");
    return;
}

var (renderer, transport) = Bootstrap.BuildServices();

var state = StateStore.Load();
var filter = "";
var onlyFav = false;

Console.WriteLine("Notification Template Tester");
Console.WriteLine("============================");
Console.WriteLine("Type 'help' to see commands.");
Console.WriteLine();

while (true)
{
    var visible = View.ApplyView(templates, filter, onlyFav, state).ToList();
    View.PrintHeader(filter, onlyFav, state);
    View.PrintTemplates(visible);

    Console.WriteLine();
    Console.Write("cmd> ");
    var cmdLine = Console.ReadLine()?.Trim();
    if (string.IsNullOrWhiteSpace(cmdLine))
        continue;

    if (Commands.TryHandleCommand(
            cmdLine,
            visible,
            templates,
            state,
            renderer,
            transport,
            ref filter,
            ref onlyFav))
        continue;
}
