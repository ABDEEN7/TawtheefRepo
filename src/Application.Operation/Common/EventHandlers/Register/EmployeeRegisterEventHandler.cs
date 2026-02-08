using Cortex.Mediator.Notifications;
using Microsoft.AspNetCore.Identity;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Domain.Events.Operation.Employee.Register;

namespace Application.Operation.Common.EventHandlers.Register;
//TODO no need for it because it causing error when save the user because of try to assign role while the user still not exists in DB.
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
