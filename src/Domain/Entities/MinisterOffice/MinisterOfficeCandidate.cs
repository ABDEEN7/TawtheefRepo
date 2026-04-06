using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.MinisterOffice;

[Table(nameof(MinisterOfficeCandidate), Schema = Schemas.Hr)]
[Index(nameof(Qid), IsUnique = true)]
public class MinisterOfficeCandidate : EventEntity
{
    [MaxLength(11)]
    public string Qid { get; set; } = default!;

    [MaxLength(20)]
    public string PhoneNumber { get; set; } = default!;

    public DateOnly QidExpiryDate { get; set; }

    /// <summary>
    /// When false the candidate is "archived" — they can be returned to follow-up later.
    /// </summary>
    public bool IsFollowUpActive { get; set; } = true;

    // ── MOI-enriched data ──────────────────────────────
    [MaxLength(200)]
    public string FullNameEn { get; set; } = default!;

    [MaxLength(200)]
    public string FullNameAr { get; set; } = default!;

    [MaxLength(100)]
    public string NationalityEn { get; set; } = default!;

    [MaxLength(100)]
    public string NationalityAr { get; set; } = default!;

    public int NationalityCode { get; set; }
}
