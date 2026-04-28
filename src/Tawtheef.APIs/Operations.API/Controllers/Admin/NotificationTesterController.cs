using System.Reflection;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Notification;
using Tawtheef.Notifications;
using Tawtheef.Notifications.Attributes;
using Tawtheef.Notifications.Interfaces;
using Tawtheef.Notifications.Utils;

namespace Operations.API.Controllers.Admin;

/// <summary>
/// Dev/Staging-only controller for testing notification templates.
/// Blocked in Production environments.
/// </summary>
[ApiController]
[Route("api/notification-tester")]
// [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "SystemAdmin")]
public class NotificationTesterController(
    IUnitOfWork unitOfWork,
    IEmailTemplateRenderer renderer,
    IWebHostEnvironment env) : ControllerBase
{
    /// <summary>
    /// Returns all registered notification templates with metadata and parameter info.
    /// </summary>
    [HttpGet("templates")]
    public IActionResult GetTemplates()
    {
        if (env.IsProduction())
            return NotFound();

        var templates = GetAllTemplateMetadata();
        return Ok(templates);
    }

    /// <summary>
    /// Creates a test Notification entity that the NotificationDispatcher background service will pick up and send.
    /// </summary>
    [HttpPost("send")]
    public async Task<IActionResult> SendTestNotification([FromBody] SendTestRequest request, CancellationToken ct)
    {
        if (env.IsProduction())
            return NotFound();

        if (string.IsNullOrWhiteSpace(request.TemplateKey))
            return BadRequest(new { error = "templateKey is required" });

        if (string.IsNullOrWhiteSpace(request.Channel))
            return BadRequest(new { error = "channel is required" });

        if (!Enum.TryParse<NotificationChannel>(request.Channel, true, out var channel))
            return BadRequest(new { error = $"Invalid channel: {request.Channel}. Use Email, Sms, or InApp." });

        try
        {
            var payloadJson = BuildPayloadJson(request.TemplateKey, request.Parameters);

            var toAddress = channel switch
            {
                NotificationChannel.Email => request.ToAddress,
                NotificationChannel.Sms => request.PhoneNumber,
                _ => request.ToAddress
            };

            Guid? parsedUserId = null;
            if (Guid.TryParse(request.UserId, out var guid)) parsedUserId = guid;

            var notification = Notification.Create(
                channel,
                request.TemplateKey,
                parsedUserId,
                toAddress,
                null, // subject – will be auto-resolved by NotificationDispatcher from template metadata
                null, // body – will be rendered by NotificationDispatcher
                null, // plainTextBody
                payloadJson,
                request.Language ?? "ar", $"TEST_{request.TemplateKey}_{channel}_{Guid.NewGuid():N}", 3);

            await unitOfWork.GetEntityRepository<Notification>().AddAsync(notification, ct);
            await unitOfWork.SaveChangesAsync(ct);

            return Ok(new
            {
                success = true,
                notificationId = notification.Id,
                message = $"Test notification queued for {channel} delivery via template '{request.TemplateKey}'."
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = $"Server Error: {ex.Message}" });
        }
    }

    /// <summary>
    /// Renders a template preview without actually sending.
    /// </summary>
    [HttpPost("preview")]
    public async Task<IActionResult> PreviewTemplate([FromBody] PreviewRequest request)
    {
        if (env.IsProduction())
            return NotFound();

        if (string.IsNullOrWhiteSpace(request.TemplateKey))
            return BadRequest(new { error = "templateKey is required" });

        var payloadJson = BuildPayloadJson(request.TemplateKey, request.Parameters);
        var language = request.Language ?? "ar";

        try
        {
            var html = await renderer.RenderHtmlAsync(request.TemplateKey, payloadJson, language);
            var text = await renderer.RenderTextAsync(request.TemplateKey, payloadJson, language);
            var subject = renderer.GetDefaultSubject(request.TemplateKey, language);

            return Ok(new PreviewResponse
            {
                Subject = subject,
                Html = html,
                PlainText = text
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = $"Failed to render template: {ex.Message}" });
        }
    }

    #region Helpers

    private static List<TemplateMetadataDto> GetAllTemplateMetadata()
    {
        var asm = typeof(NotificationAssemblyMarker).Assembly;
        var types = asm.GetTypes()
            .Where(t => t is { IsAbstract: false, IsInterface: false })
            .Select(t => new { Type = t, Attr = t.GetCustomAttribute<NotificationTemplateAttribute>() })
            .Where(x => x.Attr is not null)
            .ToList();

        var result = new List<TemplateMetadataDto>();

        foreach (var item in types)
        {
            var attr = item.Attr!;
            var modelType = item.Type;

            // Extract constructor parameters or properties as template parameters
            var parameters = GetModelParameters(modelType);

            // Determine supported channels by checking which template files exist
            var supportedChannels = GetSupportedChannels(attr.TemplateKey, asm);

            result.Add(new TemplateMetadataDto
            {
                TemplateKey = attr.TemplateKey,
                SubjectAr = attr.SubjectAr,
                SubjectEn = attr.SubjectEn,
                SupportedChannels = supportedChannels,
                Parameters = parameters
            });
        }

        return result.OrderBy(t => t.TemplateKey).ToList();
    }

    private static List<ParameterDto> GetModelParameters(Type modelType)
    {
        var parameters = new List<ParameterDto>();

        // Try constructor parameters first (for records)
        var ctor = modelType.GetConstructors()
            .OrderByDescending(c => c.GetParameters().Length)
            .FirstOrDefault();

        if (ctor != null && ctor.GetParameters().Length > 0)
        {
            foreach (var p in ctor.GetParameters())
            {
                parameters.Add(new ParameterDto
                {
                    Name = p.Name!,
                    Type = GetFriendlyTypeName(p.ParameterType),
                    IsEnum = p.ParameterType.IsEnum,
                    EnumValues = p.ParameterType.IsEnum
                        ? Enum.GetNames(p.ParameterType).ToList()
                        : null,
                    DefaultValue = GetDefaultTestValue(p.ParameterType)
                });
            }
        }
        else
        {
            // Fall back to public settable properties
            foreach (var prop in modelType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                         .Where(p => p.CanWrite && p.Name != "TemplateKey"))
            {
                parameters.Add(new ParameterDto
                {
                    Name = prop.Name,
                    Type = GetFriendlyTypeName(prop.PropertyType),
                    IsEnum = prop.PropertyType.IsEnum,
                    EnumValues = prop.PropertyType.IsEnum
                        ? Enum.GetNames(prop.PropertyType).ToList()
                        : null,
                    DefaultValue = GetDefaultTestValue(prop.PropertyType)
                });
            }
        }

        return parameters;
    }

    private static List<string> GetSupportedChannels(string templateKey, Assembly asm)
    {
        var resourceNames = asm.GetManifestResourceNames();
        var channels = new List<string>();

        // Check for HTML template → Email
        if (resourceNames.Any(r => r.Contains($"{templateKey}.html.cshtml", StringComparison.OrdinalIgnoreCase) ||
                                    r.Contains($"{templateKey}.ar.html.cshtml", StringComparison.OrdinalIgnoreCase)))
            channels.Add("Email");

        // Check for TXT template → SMS
        if (resourceNames.Any(r => r.Contains($"{templateKey}.txt.cshtml", StringComparison.OrdinalIgnoreCase) ||
                                    r.Contains($"{templateKey}.ar.txt.cshtml", StringComparison.OrdinalIgnoreCase)))
            channels.Add("Sms");

        // InApp is always supported since it just stores the notification
        channels.Add("InApp");

        return channels;
    }

    private static string GetFriendlyTypeName(Type type)
    {
        if (type == typeof(string)) return "string";
        if (type == typeof(int)) return "int";
        if (type == typeof(Guid)) return "guid";
        if (type == typeof(bool)) return "bool";
        if (type.IsEnum) return $"enum:{type.Name}";
        return type.Name;
    }

    private static string GetDefaultTestValue(Type type)
    {
        if (type == typeof(string)) return "[Test]";
        if (type == typeof(int)) return "5";
        if (type == typeof(Guid)) return Guid.Empty.ToString();
        if (type == typeof(bool)) return "true";
        if (type.IsEnum)
        {
            var values = Enum.GetValues(type);
            return values.Length > 0 ? values.GetValue(0)!.ToString()! : "0";
        }
        return "[Test]";
    }

    private string BuildPayloadJson(string templateKey, Dictionary<string, string?>? parameters)
    {
        // Get the model type for this template to know what parameters are expected
        var modelParams = new Dictionary<string, object?>();

        try
        {
            var modelType = NotificationTemplateRegistry.GetModelTypeFor(templateKey);
            var expectedParams = GetModelParameters(modelType);

            foreach (var expected in expectedParams)
            {
                var value = parameters?.GetValueOrDefault(expected.Name) ?? expected.DefaultValue;
                modelParams[expected.Name] = ConvertParameterValue(expected.Type, value);
            }
        }
        catch (KeyNotFoundException)
        {
            // Template not in registry; use provided parameters as-is
            if (parameters != null)
            {
                foreach (var kvp in parameters)
                    modelParams[kvp.Key] = kvp.Value;
            }
        }

        return JsonSerializer.Serialize(modelParams);
    }

    private static object? ConvertParameterValue(string typeName, string? value)
    {
        if (value == null) return null;
        if (typeName == "int" && int.TryParse(value, out var intVal)) return intVal;
        if (typeName == "guid" && Guid.TryParse(value, out var guidVal)) return guidVal;
        if (typeName == "bool" && bool.TryParse(value, out var boolVal)) return boolVal;
        if (typeName.StartsWith("enum:") && int.TryParse(value, out var enumVal)) return enumVal;
        return value;
    }

    #endregion

    #region DTOs

    public class SendTestRequest
    {
        public string TemplateKey { get; set; } = string.Empty;
        public string Channel { get; set; } = string.Empty;
        public string? ToAddress { get; set; }
        public string? PhoneNumber { get; set; }
        public string? UserId { get; set; }
        public string? Language { get; set; } = "ar";
        public Dictionary<string, string?>? Parameters { get; set; }
    }

    public class PreviewRequest
    {
        public string TemplateKey { get; set; } = string.Empty;
        public string? Language { get; set; } = "ar";
        public Dictionary<string, string?>? Parameters { get; set; }
    }

    public class PreviewResponse
    {
        public string Subject { get; set; } = string.Empty;
        public string Html { get; set; } = string.Empty;
        public string PlainText { get; set; } = string.Empty;
    }

    public class TemplateMetadataDto
    {
        public string TemplateKey { get; set; } = string.Empty;
        public string SubjectAr { get; set; } = string.Empty;
        public string SubjectEn { get; set; } = string.Empty;
        public List<string> SupportedChannels { get; set; } = [];
        public List<ParameterDto> Parameters { get; set; } = [];
    }

    public class ParameterDto
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public bool IsEnum { get; set; }
        public List<string>? EnumValues { get; set; }
        public string DefaultValue { get; set; } = string.Empty;
    }

    #endregion
}
