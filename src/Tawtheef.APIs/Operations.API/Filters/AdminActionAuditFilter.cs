using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Domain.Entities.Logger;
using Tawtheef.Infrastructure.Data;

namespace Operations.API.Filters;

public sealed class AdminActionAuditFilter(
    TawtheefDbContext dbContext,
    ICurrentUserService currentUserService,
    ILogger<AdminActionAuditFilter> logger) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var executedContext = await next();

        if (executedContext.Exception is not null || executedContext.Canceled)
            return;

        var request = context.HttpContext.Request;
        if (HttpMethods.IsGet(request.Method))
            return;

        if (context.ActionDescriptor is not ControllerActionDescriptor controllerAction)
            return;

        if (!controllerAction.ControllerTypeInfo.Namespace?.Contains(".Controllers.Admin", StringComparison.Ordinal) ?? true)
            return;

        if (context.HttpContext.Response.StatusCode is < 200 or >= 300)
            return;

        if (!Guid.TryParse(currentUserService.UserId, out var userId))
            return;

        try
        {
            var payload = BuildPayload(context.ActionArguments);
            var section = controllerAction.ControllerName;
            var action = controllerAction.ActionName;
            var entityId = ResolveEntityId(context.ActionArguments);

            var entry = new ActionLog
            {
                UserProfileId = Guid.Empty,
                UserId = userId,
                LogType = ActionLogType.Admin,
                ActionType = $"{section}.{action}",
                Section = section,
                Notes = payload,
                EntityId = entityId,
                AttachmentId = null,
                CreatedDate = DateTime.UtcNow
            };

            dbContext.Set<ActionLog>().Add(entry);
            await dbContext.SaveChangesAsync(context.HttpContext.RequestAborted);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to persist admin action audit entry for {Action}", controllerAction.ActionName);
        }
    }

    private static string? BuildPayload(IDictionary<string, object?> actionArguments)
    {
        if (actionArguments.Count == 0)
            return null;

        try
        {
            return JsonSerializer.Serialize(actionArguments);
        }
        catch
        {
            return null;
        }
    }

    private static Guid? ResolveEntityId(IDictionary<string, object?> actionArguments)
    {
        foreach (var (_, value) in actionArguments)
        {
            if (value is Guid guid && guid != Guid.Empty)
                return guid;

            if (value is string text && Guid.TryParse(text, out var stringGuid) && stringGuid != Guid.Empty)
                return stringGuid;

            if (value is null)
                continue;

            var idProperty = value.GetType().GetProperty("Id");
            if (idProperty?.GetValue(value) is Guid propertyGuid && propertyGuid != Guid.Empty)
                return propertyGuid;
        }

        return null;
    }
}
