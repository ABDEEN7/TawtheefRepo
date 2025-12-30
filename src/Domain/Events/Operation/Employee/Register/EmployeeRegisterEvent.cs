using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Events.Operation.Employee.Register;

public record EmployeeRegisterEvent(Guid EmployeeId, string FullName, string Email, DateTimeOffset DateOccurred) : BaseEvent(DateOccurred);
