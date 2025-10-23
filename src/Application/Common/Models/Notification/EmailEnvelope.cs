using System.Collections.Generic;

namespace Tawtheef.Application.Common.Models.Notification;

public sealed record EmailEnvelope(
    List<string> To,
    List<string>? Cc,
    string Subject,
    string? HtmlBody,
    string? PlainTextBody
);
