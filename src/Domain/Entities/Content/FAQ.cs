using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Content;

[Table(nameof(FAQ), Schema = Schemas.Applicant)]
public sealed class FAQ : EventEntity
{
    [Required]
    [MaxLength(400)]
    public string QuestionAr { get; set; } = string.Empty;

    [Required]
    [MaxLength(400)]
    public string QuestionEn { get; set; } = string.Empty;

    [Required]
    [MaxLength(1000)]
    public string AnswerAr { get; set; } = string.Empty;

    [Required]
    [MaxLength(1000)]
    public string AnswerEn { get; set; } = string.Empty;

    public int DisplayOrder { get; set; }

    public bool IsActive { get; set; }
}
