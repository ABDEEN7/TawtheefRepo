using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Events.Operation.Employee.Register;
//TODO no need for it because it causing error when save the user because of try to assign role while the user still not exists in DB.
public record EmployeeRegisterEvent(Guid EmployeeId, string FullName, string Email, DateTimeOffset DateOccurred) : BaseEvent(DateOccurred);
