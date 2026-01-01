namespace Tawtheef.Domain.Entities.Security;


/// <summary>
/// Strongly-typed permission key.
/// Persisted into claims/policies as string values (PermissionKey.Value).
/// </summary>
public readonly record struct PermissionKey(string Value)
{
    public override string ToString() => Value;
}
