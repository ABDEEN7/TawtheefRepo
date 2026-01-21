namespace Tawtheef.Notifications.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class NotificationTemplateAttribute : Attribute
{
    public string TemplateKey { get; }

    public NotificationTemplateAttribute(string templateKey)
        => TemplateKey = templateKey;
}
