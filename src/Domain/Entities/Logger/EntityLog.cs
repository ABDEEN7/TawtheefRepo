using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Domain.Entities.Logger;

public class EntityLog : EventEntity
{
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public required string EntityType { get; init; }
    public Guid EntityId { get; init; }
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public required string Action { get; init; }
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? ActionDetails { get; init; }
    public DateTime ChangeDate { get; init; } = DateTime.UtcNow;
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? OldValues { get; set; }
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? NewValues { get; set; }
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    // ReSharper disable once InconsistentNaming
    public string? IPAddress { get; init; }
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? UserAgent { get; init; }
    public Guid? ChangedByUserId { get; init; }
    public User? ChangedByUser { get; init; }
}
