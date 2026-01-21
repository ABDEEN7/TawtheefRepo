namespace Tawtheef.Notifications.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class NotificationTemplateAttribute(string templateKey) : Attribute
{
    public string TemplateKey { get; } = templateKey;
}
