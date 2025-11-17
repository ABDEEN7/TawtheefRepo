using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Users;

/// <summary>
/// Represents a submitted snapshot of a user profile at a specific point in time.
/// Stores the complete profile data (including all related collections) as JSON for historical tracking.
/// The version increments with each submission.
/// </summary>
public class ProfileSubmission : EventEntity
{
    public Guid UserProfileId { get; set; }
    /// <summary>
    /// Increments with each submission
    /// </summary>
    public int Version { get; set; }
    public DateTime SubmittedAtUtc { get; set; }
    /// <summary>
    /// Complete JSON snapshot (Profile + Collections)
    /// </summary>
    public string SnapshotJson { get; set; } = default!;
}
