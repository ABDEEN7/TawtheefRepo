using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Exams;

[Table(nameof(Location), Schema = Schemas.Hr)]
public class Location : EventEntity
{
    public required string NameAr { get; set; }
    public string? NameEn { get; set; }
    public required string LocationLink { get; set; }
    public string? Notes { get; set; }
}
