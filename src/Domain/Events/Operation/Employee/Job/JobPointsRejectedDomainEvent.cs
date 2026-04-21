using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Recruitment;

namespace Tawtheef.Domain.Events.Operation.Employee.Job;

public sealed record JobPointsRejectedDomainEvent(Tawtheef.Domain.Entities.Recruitment.Job Job, string Reason, DateTimeOffset OnDateOccurred)
    : BaseEvent(OnDateOccurred);
