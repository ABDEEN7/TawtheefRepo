using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Content;

[Table(nameof(HomeSuccessStory), Schema = Schemas.Applicant)]
public sealed class HomeSuccessStory : EventEntity
{
    [Required]
    [MaxLength(200)]
    public string NameAr { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string NameEn { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string RoleAr { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string RoleEn { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string MetricTitleAr { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string MetricTitleEn { get; set; } = string.Empty;

    [Required]
    [MaxLength(400)]
    public string MetricDescriptionAr { get; set; } = string.Empty;

    [Required]
    [MaxLength(400)]
    public string MetricDescriptionEn { get; set; } = string.Empty;

    [Required]
    [MaxLength(2048)]
    public string ImageUrl { get; set; } = string.Empty;

    public int DisplayOrder { get; set; }

    public bool IsActive { get; set; }
}
