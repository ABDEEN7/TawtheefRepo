using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Domain.Entities.Recruitment;

public enum ProfileSection
{
    Personal = 1,
    BasicInformation = Personal,
    Contact = 2,
    Qualifications = 3,
    Experience = 4,
    Experiences = Experience,
    SkillsLanguages = 5,
    SkillsAndLanguages = SkillsLanguages,
    Attachments = 6,
    TrainingCourses = 7,
    ProfessionalCertificatesAndAwards = 8,
    ProfilePhoto = 9
}

public enum ReviewStatus
{
    NotReviewed = 0,
    Pending = 1,
    Approved = 2,
    Rejected = 3,
    NeedsCorrection = 4,
    ChangesRequested = NeedsCorrection
}
public enum ReviewTargetType { Section=1, Field=2, Row=3, Attachment=4 }

[Table(nameof(ReviewItem), Schema = Schemas.Hr)]
public class ReviewItem : EventEntity
{
    /// <summary>
    /// The pending or approved change that this review item is validating
    /// </summary>
    public Guid? ProfileChangeId { get; set; }
    public ProfileChange? ProfileChange { get; set; }

    /// <summary>
    /// The user profile being reviewed
    /// </summary>
    public Guid UserProfileId { get; set; }
    public UserProfile? UserProfile { get; set; }

    /// <summary>
    /// Submission version at the time the review was sent
    /// </summary>
    public int Version { get; set; }
    
    /// <summary>
    /// Type of target being reviewed (Section, Field, Row, or Attachment)
    /// </summary>
    public ReviewTargetType TargetType { get; set; }
    
    /// <summary>
    /// The profile section/tab that the target belongs to
    /// </summary>
    public ProfileSection Section { get; set; }
    
    /// <summary>
    /// Path to the field being reviewed. Examples: "FullNameEn" or "ResidenceAddress.Zone"
    /// </summary>
    public string? FieldPath { get; set; }
    /// <summary>
    /// Entity name for table items. Examples: "Qualification", "Experience", "TrainingCourse"
    /// </summary>
    public string? EntityName { get; set; }
    
    /// <summary>
    /// The ID of the targeted row/entity
    /// </summary>
    public Guid? EntityId { get; set; }
    
    /// <summary>
    /// Resource ID for attachments. Examples: ResumeAttachmentId, AdditionalAttachments.AttachmentId
    /// </summary>
    public Guid? ResourceId { get; set; }
    
    /// <summary>
    /// Clear title for the attachment when needed
    /// </summary>
    public string? AttachmentTitle { get; set; }
    
    /// <summary>
    /// Current status of the review (Pending, Approved, Rejected, etc.)
    /// </summary>
    public ReviewStatus Status { get; set; } = ReviewStatus.Pending;
    
    /// <summary>
    /// ID of the user who performed the review
    /// </summary>
    public Guid? ReviewedById { get; set; }
    
    /// <summary>
    /// Timestamp when the review was completed
    /// </summary>
    public DateTime? ReviewedAtUtc { get; set; }
    
    /// <summary>
    /// Notes or comments from the reviewer
    /// </summary>
    public string? ReviewerNote { get; set; }
    
    
    /// <summary>
    /// Hash value at the time of approval
    /// </summary>
    public string? ApprovedHash { get; set; }
    
    /// <summary>
    /// Latest calculated hash from user data
    /// </summary>
    public string? CurrentHash { get; set; }
    
    /// <summary>
    /// Indicates whether the data has been modified since approval
    /// </summary>
    public bool IsOutdated { get; set; }

    /// <summary>
    /// Submission version at the time of approval
    /// </summary>
    public int? ApprovedAtVersion { get; set; }
    
    
    public static ReviewItem Create(Guid userProfileId, ProfileSection section,
        ReviewTargetType targetType, string? fieldPath = null, string? entityName = null,
        Guid? entityId = null, Guid? resourceId = null, object? currentValue = null)
    {
        var item = new ReviewItem
        {
            UserProfileId = userProfileId,
            Section = section,
            TargetType = targetType,
            FieldPath = fieldPath,
            EntityName = entityName,
            EntityId = entityId,
            ResourceId = resourceId,
            Version = 1
        };

        item.UpdateHash(currentValue);
        return item;
    }

    public void UpdateHash(object? newValue)
    {
        CurrentHash = ComputeHash(newValue);

        if (ApprovedHash == null)
        {
            // لم يُعتمد بعد — يظل Pending
            Status = ReviewStatus.Pending;
            IsOutdated = true;
            return;
        }

        if (ApprovedHash != CurrentHash)
        {
            // تغيّرت القيمة
            Status = ReviewStatus.Pending;
            IsOutdated = true;
        }
        else
        {
            // نفس القيمة السابقة
            IsOutdated = false;
        }
    }

    public void Approve(Guid reviewerId, string? note = null)
    {
        ReviewedById = reviewerId;
        ReviewedAtUtc = DateTime.UtcNow;
        ReviewerNote = note;
        ApprovedHash = CurrentHash;
        IsOutdated = false;
        Status = ReviewStatus.Approved;
    }

    public void Reject(Guid reviewerId, string reason)
    {
        ReviewedById = reviewerId;
        ReviewedAtUtc = DateTime.UtcNow;
        ReviewerNote = reason;
        Status = ReviewStatus.Rejected;
        IsOutdated = false;
    }

    public void RequestChanges(Guid reviewerId, string message)
    {
        ReviewedById = reviewerId;
        ReviewedAtUtc = DateTime.UtcNow;
        ReviewerNote = message;
        Status = ReviewStatus.NeedsCorrection;
    }

    public bool NeedsReview() => IsOutdated || Status is ReviewStatus.Pending or ReviewStatus.NeedsCorrection;

    // 🔐 Utility for Hash Calculation
    private static string ComputeHash(object? value)
    {
        if (value == null) return string.Empty;
        var json = value is string s ? s : JsonSerializer.Serialize(value, new JsonSerializerOptions { WriteIndented = false });
        using var sha = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(json.Trim());
        return Convert.ToHexString(sha.ComputeHash(bytes));
    }
}
