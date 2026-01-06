using Tawtheef.Domain.Common;
using JobEntity = Tawtheef.Domain.Entities.Recruitment.Job;

namespace Tawtheef.Domain.Events.Operation.Employee.Job;

public sealed record JobUpdatedDomainEvent(
    JobEntity Job,
    DateTimeOffset DateOccurred) : BaseEvent(DateOccurred);
