
using System.Globalization;
using System.Net.Mail;
using System.Reflection;
using Microsoft.Extensions.Options;
using Tawtheef.Application.Common.Interfaces.NotificationServices;
using Tawtheef.Application.Common.Models.Notification;
using Tawtheef.Domain.Configurations.Settings;
using Tawtheef.Notifications;
using Tawtheef.Notifications.Attributes;
using Tawtheef.Notifications.Context;
using Tawtheef.Notifications.Interfaces;
using Tawtheef.Notifications.Services;
using Tawtheef.Notifications.Utils;

NotificationTemplateRegistry.AutoRegisterFrom(typeof(NotificationAssemblyMarker).Assembly);

var templates = typeof(NotificationAssemblyMarker).Assembly
    .GetTypes()
    .Where(type => type is { IsAbstract: false, IsInterface: false })
    .Select(type => new TemplateEntry(type, type.GetCustomAttribute<NotificationTemplateAttribute>()!))
    .Where(entry => entry.Attribute is not null)
    .Select(entry => entry with { Attribute = entry.Attribute! })
    .OrderBy(entry => entry.Attribute.TemplateKey)
    .ToList();

if (templates.Count == 0)
{
    Console.WriteLine("No notification templates found.");
    return;
}

var emailSettings = new EmailSettings
{
    SmtpHost = "smtp.edu.gov.qa",
    SmtpPort = 25,
    EmailUser = "tawtheef@edu.gov.qa",
    EmailPass = "Taw@Theef",
    ManagerEmails = "manager@tawtheef.local",
    ContactUsEmail = "contact@tawtheef.local",
    ProductName = "Tawtheef"
};

var appConfig = new AppConfigSettings
{
    FrontendUrl = "https://localhost",
    BackendUrl = "https://localhost",
    BlobSignKey = "dev",
    AdminEmail = "admin@tawtheef.local",
    AdminEmails = ["admin@tawtheef.local"],
    DefaultSignedUrlMinutes = 3
};
var branding = new DefaultBranding(Options.Create(emailSettings), Options.Create(appConfig));
var renderer = new RazorTemplateRenderer(branding);
var transport = new FileEmailTransport(emailSettings);

Console.WriteLine("Notification Template Tester");
Console.WriteLine("============================");
Console.WriteLine();

while (true)
{
    Console.WriteLine("Templates:");
    for (var i = 0; i < templates.Count; i++)
    {
        Console.WriteLine($"  {i + 1}. {templates[i].Attribute!.TemplateKey}");
    }

    Console.WriteLine();
    Console.Write("Choose a template number, 'all', or 'q' to quit: ");
    var choice = Console.ReadLine()?.Trim();

    if (string.Equals(choice, "q", StringComparison.OrdinalIgnoreCase))
        break;

    if (string.Equals(choice, "all", StringComparison.OrdinalIgnoreCase))
    {
        foreach (var template in templates)
            await SendTemplateAsync(template, renderer, transport);

        continue;
    }

    if (int.TryParse(choice, out var index) && index >= 1 && index <= templates.Count)
    {
        await SendTemplateAsync(templates[index - 1], renderer, transport);
        continue;
    }

    Console.WriteLine("Invalid option. Try again.");
}

static async Task SendTemplateAsync(
    TemplateEntry templateEntry,
    IEmailTemplateRenderer renderer,
    IEmailTransport transport)
{
    var templateKey = templateEntry.Attribute.TemplateKey;
    var modelType = templateEntry.Type;

    Console.WriteLine();
    Console.WriteLine($"Preparing template: {templateKey}");

    var subject = Prompt("Subject", templateKey, required: true);
    var to = PromptList("To emails (comma-separated)", required: true);
    var cc = PromptList("CC emails (comma-separated)", required: false);

    var model = BuildModel(modelType);

    Console.WriteLine("Rendering template...");
    var html = await renderer.RenderHtmlAsync(templateKey, (dynamic)model);
    var text = await renderer.RenderTextAsync(templateKey, (dynamic)model);

    var envelope = new EmailEnvelope(to, cc, subject, html, text);
    await transport.SendAsync(envelope);

    Console.WriteLine("Template sent.");
    Console.WriteLine();
}

static object BuildModel(Type modelType)
{
    var constructor = modelType
        .GetConstructors()
        .OrderByDescending(ctor => ctor.GetParameters().Length)
        .FirstOrDefault();

    if (constructor is null)
        throw new InvalidOperationException($"No public constructor found for {modelType.FullName}.");

    var parameters = constructor.GetParameters();
    if (parameters.Length == 0)
        return Activator.CreateInstance(modelType)
               ?? throw new InvalidOperationException($"Unable to create instance of {modelType.FullName}.");

    var args = new object?[parameters.Length];

    for (var i = 0; i < parameters.Length; i++)
    {
        var parameter = parameters[i];
        var isNullable = !parameter.ParameterType.IsValueType || Nullable.GetUnderlyingType(parameter.ParameterType) is not null;
        var defaultValue = parameter.HasDefaultValue ? parameter.DefaultValue : null;

        var promptText = $"{parameter.Name} ({parameter.ParameterType.Name})";
        var input = Prompt(promptText, defaultValue?.ToString(), required: !isNullable && defaultValue is null);

        if (string.IsNullOrWhiteSpace(input))
        {
            args[i] = defaultValue;
            continue;
        }

        args[i] = ConvertInput(input, parameter.ParameterType);
    }

    return constructor.Invoke(args);
}

static string Prompt(string label, string? defaultValue = null, bool required = true)
{
    while (true)
    {
        var suffix = string.IsNullOrWhiteSpace(defaultValue) ? string.Empty : $" [{defaultValue}]";
        Console.Write($"{label}{suffix}: ");
        var input = Console.ReadLine();

        if (!string.IsNullOrWhiteSpace(input))
            return input.Trim();

        if (!string.IsNullOrWhiteSpace(defaultValue))
            return defaultValue;

        if (!required)
            return string.Empty;

        Console.WriteLine("This field is required.");
    }
}

static List<string> PromptList(string label, bool required)
{
    while (true)
    {
        var input = Prompt(label, required: required);
        var values = input.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();

        if (values.Count > 0 || !required)
            return values;

        Console.WriteLine("At least one value is required.");
    }
}

static object? ConvertInput(string value, Type targetType)
{
    var nonNullable = Nullable.GetUnderlyingType(targetType) ?? targetType;

    if (nonNullable == typeof(string))
        return value;

    if (nonNullable == typeof(Guid))
        return Guid.Parse(value);

    if (nonNullable == typeof(DateTime))
        return DateTime.Parse(value, CultureInfo.InvariantCulture);

    if (nonNullable == typeof(DateOnly))
        return DateOnly.Parse(value, CultureInfo.InvariantCulture);

    if (nonNullable == typeof(TimeOnly))
        return TimeOnly.Parse(value, CultureInfo.InvariantCulture);

    if (nonNullable.IsEnum)
        return Enum.Parse(nonNullable, value, ignoreCase: true);

    return Convert.ChangeType(value, nonNullable, CultureInfo.InvariantCulture);
}

sealed class FileEmailTransport(EmailSettings emailSettings) : IEmailTransport
{
    public Task SendAsync(EmailEnvelope envelope, CancellationToken ct = default)
    {
        using var message = new MailMessage
        {
            From = new MailAddress(emailSettings.EmailUser),
            Subject = envelope.Subject,
            Body = envelope.HtmlBody,
            IsBodyHtml = true,
        };

        foreach(var to in envelope.To)
            message.To.Add(to);

        foreach(var cc in envelope.Cc ?? [])
            message.To.Add(cc);

        using var smtp = new SmtpClient(emailSettings.SmtpHost, emailSettings.SmtpPort)
        {
            EnableSsl = true,
            //UseDefaultCredentials = false
            // IMPORTANT: no Credentials set
        };

        smtp.Send(message);
        return Task.CompletedTask;
    }
}

file sealed record TemplateEntry(Type Type, NotificationTemplateAttribute Attribute);
