using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Recruitment.JobDetails;

[Table(nameof(JobRequiredAttachment), Schema = Schemas.Hr)]
public class JobRequiredAttachment : EventEntity
{
    public Guid JobId { get; set; }
    public Job? Job { get; set; }

    [MaxLength(200)]
    public required string TitleAr { get; set; }
    [MaxLength(200)]
    public required string TitleEn { get; set; }
    public bool IsMandatory { get; set; }
}
