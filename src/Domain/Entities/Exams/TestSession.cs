using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Domain.Entities.Exams;

[Table(nameof(TestSession), Schema = Schemas.Hr)]
public class TestSession : EventEntity
{
    public Guid TestSlotId { get; set; }
    public Guid ExamId { get; set; }
    public int SessionNo { get; set; }
    public Guid StatusId { get; set; }

    public TestSlot? TestSlot { get; set; }
    public Exam? Exam { get; set; }
    public TestSessionStatus? Status { get; set; }
}
