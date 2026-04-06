using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.MinisterOffice;

[Table(nameof(MinisterOfficeCandidateAuditLog), Schema = Schemas.Hr)]
[Index(nameof(CandidateId))]
[Index(nameof(Action))]
public class MinisterOfficeCandidateAuditLog : EventEntity
{
    public Guid CandidateId { get; set; }
    public MinisterOfficeCandidate? Candidate { get; set; }

    [MaxLength(100)]
    public string Action { get; set; } = default!;

    [MaxLength(2000)]
    public string? Details { get; set; }

    [MaxLength(11)]
    public string? Qid { get; set; }
}
