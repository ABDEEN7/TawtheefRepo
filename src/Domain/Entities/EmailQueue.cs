using System;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities;

public class EmailQueue : EventEntity
{
    public required string RecipientEmail { get; set; }
    public required string Subject { get; set; }
    public required string Body { get; set; }
    public bool IsSent { get; set; } = false;
    public DateTime? SentDate { get; set; }
    public int RetryCount { get; set; } = 0;
    public string? ErrorMessage { get; set; }
}
