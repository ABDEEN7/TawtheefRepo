namespace Tawtheef.Notifications.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class NotificationTemplateAttribute(string templateKey, string subjectAr = "", string subjectEn = "") : Attribute
{
    public string TemplateKey { get; } = templateKey;
    public string SubjectAr { get; } = subjectAr;
    public string SubjectEn { get; } = subjectEn;
}
