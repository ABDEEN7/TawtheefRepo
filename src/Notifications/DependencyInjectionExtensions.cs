using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tawtheef.Notifications.Utils;

namespace Tawtheef.Notifications;

public static class DependencyInjectionExtensions
{
    public static void AddNotificationLayer(this IServiceCollection _,IConfiguration __)
    {
        NotificationTemplateRegistry.RegisterFromAssembly(typeof(NotificationAssemblyMarker).Assembly);
    }
}
