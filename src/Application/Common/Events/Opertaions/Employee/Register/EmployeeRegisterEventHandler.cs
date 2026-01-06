using Cortex.Mediator.Notifications;
using Microsoft.AspNetCore.Identity;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Domain.Events.Operation.Employee.Register;

namespace Tawtheef.Application.Common.Events.Opertaions.Employee.Register;

public class EmployeeRegisterEventHandler(UserManager<User> userManager) : INotificationHandler<EmployeeRegisterEvent>
{
    public async Task Handle(EmployeeRegisterEvent notification, CancellationToken cancellationToken)
    {
        // Assign sys employee role to the newly registered employee
        var user = await userManager.FindByIdAsync(notification.EmployeeId.ToString());
        if (user != null)
        {
            await userManager.AddToRoleAsync(user, nameof(SystemRoleIds.Employee));
        }
    }
}
