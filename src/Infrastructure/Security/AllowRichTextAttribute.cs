namespace Tawtheef.Infrastructure.Security;

/// <summary>
/// Marks an endpoint whose request body intentionally contains user-authored rich text.
/// The content must still be sanitized before it is persisted.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class AllowRichTextAttribute : Attribute;
