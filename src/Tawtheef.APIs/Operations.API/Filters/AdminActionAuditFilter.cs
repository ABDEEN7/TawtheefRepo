using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Domain.Entities.Logger;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;
using Tawtheef.Infrastructure.Data;

namespace Operations.API.Filters;

public sealed class AdminActionAuditFilter(
    TawtheefDbContext dbContext,
    ICurrentUserService currentUserService,
    ILogger<AdminActionAuditFilter> logger) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.ActionDescriptor is not ControllerActionDescriptor controllerAction)
        {
            await next();
            return;
        }

        var section = controllerAction.ControllerName;
        var action = controllerAction.ActionName;
        var isUpdate = action.StartsWith("Update", StringComparison.OrdinalIgnoreCase) || 
                       action.StartsWith("Block", StringComparison.OrdinalIgnoreCase) ||
                       action.StartsWith("Set", StringComparison.OrdinalIgnoreCase);

        var entityId = ResolveEntityId(context.ActionArguments);
        Dictionary<string, object?>? oldValues = null;

        // Pre-capture old state for updates
        if (isUpdate && entityId.HasValue && entityId != Guid.Empty)
        {
            oldValues = await FetchEntityAsDictionaryAsync(section, entityId.Value);
        }

        var executedContext = await next();

        if (executedContext.Exception is not null || executedContext.Canceled)
            return;

        var request = context.HttpContext.Request;
        if (HttpMethods.IsGet(request.Method))
            return;

        if (!controllerAction.ControllerTypeInfo.Namespace?.Contains(".Controllers.Admin", StringComparison.Ordinal) ?? true)
            return;

        if (context.HttpContext.Response.StatusCode is < 200 or >= 300)
            return;

        if (!Guid.TryParse(currentUserService.UserId, out var userId))
            return;

        try
        {
            // Build human note and detect changes
            var humanNote = await BuildHumanNoteAsync(section, action, context.ActionArguments);
            var changes = DetectChanges(oldValues, context.ActionArguments);
            
            var payload = BuildPayload(context.ActionArguments);
            
            // Build the final note with Old vs New info
            var notesBuilder = new System.Text.StringBuilder();
            if (!string.IsNullOrEmpty(humanNote)) notesBuilder.AppendLine(humanNote);
            if (!string.IsNullOrEmpty(changes)) 
            {
                notesBuilder.AppendLine("--- Changes ---");
                notesBuilder.AppendLine(changes);
            }
            notesBuilder.Append($"| Details: {payload}");

            var entry = new ActionLog
            {
                UserProfileId = Guid.Empty,
                UserId = userId,
                LogType = ActionLogType.Admin,
                ActionType = $"{section}.{action}",
                Section = section,
                Notes = notesBuilder.ToString(),
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

    private async Task<Dictionary<string, object?>?> FetchEntityAsDictionaryAsync(string section, Guid id)
    {
        try
        {
            object? entity = section switch
            {
                "Offices" => await dbContext.Set<Office>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id),
                "UserManagement" or "Users" => await dbContext.Set<Tawtheef.Domain.Entities.Users.User>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id),
                "Faqs" or "HomeContent" => await dbContext.Set<Tawtheef.Domain.Entities.Content.FAQ>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id),
                "Universities" => await dbContext.Set<University>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id),
                "JobTitles" => await dbContext.Set<JobTitle>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id),
                _ => null
            };

            if (entity == null) return null;

            return entity.GetType().GetProperties()
                .Where(p => p.CanRead)
                .ToDictionary(p => p.Name, p => p.GetValue(entity), StringComparer.OrdinalIgnoreCase);
        }
        catch
        {
            return null;
        }
    }

    private string? DetectChanges(Dictionary<string, object?>? oldValues, IDictionary<string, object?> args)
    {
        if (oldValues == null || args == null) return null;

        var changeList = new List<string>();
        
        // Find the command object in args
        var command = args.Values.FirstOrDefault(v => v != null && v.GetType().IsClass && v.GetType() != typeof(string));
        if (command == null) return null;

        var commandProps = command.GetType().GetProperties();
        foreach (var cmdProp in commandProps)
        {
            if (oldValues.TryGetValue(cmdProp.Name, out var oldValue))
            {
                var newValue = cmdProp.GetValue(command);
                
                // Compare values
                if (!Equals(oldValue, newValue))
                {
                    // Skip technical IDs and dates
                    if (cmdProp.Name.EndsWith("Id") || cmdProp.Name == "Id") continue;
                    
                    var oldStr = oldValue?.ToString() ?? "None";
                    var newStr = newValue?.ToString() ?? "None";
                    
                    if (oldStr == newStr) continue;

                    changeList.Add($"{cmdProp.Name}: {oldStr} -> {newStr}");
                }
            }
        }

        return changeList.Count > 0 ? string.Join("\n", changeList) : null;
    }

    private async Task<string?> BuildHumanNoteAsync(string section, string action, IDictionary<string, object?> args)
    {
        try
        {
            var targetName = await ResolveTargetNameAsync(section, args);
            var actionLabel = action.Replace("Office", "").Replace("Faq", "").Replace("Language", "").Replace("Religion", "").Replace("TargetEntity", "").Replace("University", "");
            
            // Split camel case for the action part (e.g. CreateOffice -> Create)
            actionLabel = System.Text.RegularExpressions.Regex.Replace(actionLabel, "([A-Z])", " $1").Trim();

            if (!string.IsNullOrEmpty(targetName))
            {
                return $"{actionLabel}: {targetName}";
            }

            return null;
        }
        catch
        {
            return null;
        }
    }

    private async Task<string?> ResolveTargetNameAsync(string section, IDictionary<string, object?> args)
    {
        // 1. Try to find a name/title directly in the command argument
        foreach (var arg in args.Values)
        {
            if (arg == null) continue;
            var type = arg.GetType();
            
            // Common name properties
            var nameProps = new[] { "NameEn", "NameAr", "FullNameEn", "FullNameAr", "QuestionEn", "TitleEn" };
            foreach (var propName in nameProps)
            {
                var prop = type.GetProperty(propName);
                if (prop != null)
                {
                    var val = prop.GetValue(arg)?.ToString();
                    if (!string.IsNullOrEmpty(val)) return val;
                }
            }
        }

        // 2. Resolve IDs from database if we have an ID but no name in the payload (e.g. Delete or Update where only ID is sent)
        var id = ResolveEntityId(args);
        if (!id.HasValue || id == Guid.Empty) return null;

        return section switch
        {
            "Offices" => await dbContext.Set<Office>().Where(x => x.Id == id).Select(x => x.NameEn).FirstOrDefaultAsync(),
            "UserManagement" or "Users" => await ResolveUserOrRoleSummaryAsync(id.Value, args),
            "Faqs" or "HomeContent" => await dbContext.Set<Tawtheef.Domain.Entities.Content.FAQ>().Where(x => x.Id == id).Select(x => x.QuestionEn).FirstOrDefaultAsync(),
            "Universities" => await dbContext.Set<University>().Where(x => x.Id == id).Select(x => x.NameEn).FirstOrDefaultAsync(),
            "JobTitles" => await dbContext.Set<JobTitle>().Where(x => x.Id == id).Select(x => x.JobNameEn).FirstOrDefaultAsync(),
            _ => null
        };
    }

    private async Task<string?> ResolveUserOrRoleSummaryAsync(Guid id, IDictionary<string, object?> args)
    {
        // Check if we are updating roles
        if (args.Values.Any(v => v?.GetType().Name.Contains("UpdateUserRoles", StringComparison.OrdinalIgnoreCase) ?? false))
        {
            var user = await dbContext.Set<Tawtheef.Domain.Entities.Users.User>().Where(x => x.Id == id).Select(x => x.FullNameEn).FirstOrDefaultAsync();
            
            // Try to find RoleIds in the command
            foreach (var arg in args.Values)
            {
                if (arg == null) continue;
                var roleIdsProp = arg.GetType().GetProperty("RoleIds");
                if (roleIdsProp?.GetValue(arg) is IEnumerable<Guid> roleIds)
                {
                    var roleNames = await dbContext.Roles
                        .Where(r => roleIds.Contains(r.Id))
                        .Select(r => r.Name)
                        .ToListAsync();
                    
                    return $"{user} (Roles: {string.Join(", ", roleNames)})";
                }
            }
            return user;
        }

        return await dbContext.Set<Tawtheef.Domain.Entities.Users.User>().Where(x => x.Id == id).Select(x => x.FullNameEn).FirstOrDefaultAsync();
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

            var idProperty = value.GetType().GetProperty("Id") ?? value.GetType().GetProperty("officeId") ?? value.GetType().GetProperty("userId");
            if (idProperty?.GetValue(value) is Guid propertyGuid && propertyGuid != Guid.Empty)
                return propertyGuid;
        }

        return null;
    }
}
