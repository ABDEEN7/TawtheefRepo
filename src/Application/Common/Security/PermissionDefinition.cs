using Tawtheef.Domain.Entities.Security;

namespace Tawtheef.Application.Common.Security;

/// <summary>
/// Single source-of-truth definition used for:
/// - Policies
/// - Permission lookup seed
/// - Validation
/// - Role claims seed
/// </summary>
public sealed record PermissionDefinition(
    PermissionKey Key,
    string Module,
    PermissionAction Action,
    string NameEn,
    string NameAr,
    int DisplayOrder,
    bool CanBeAssignedToRole = true
);
