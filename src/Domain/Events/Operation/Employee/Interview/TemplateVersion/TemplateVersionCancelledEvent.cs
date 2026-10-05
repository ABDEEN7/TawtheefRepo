using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Interview;

namespace Tawtheef.Domain.Events.Operation.Employee.Interview.TemplateVersion;

public sealed record TemplateVersionCancelledEvent(InterviewTemplateVersion Version, DateTimeOffset OnDateOccurred)
    : BaseEvent(OnDateOccurred);
