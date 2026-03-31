using System.Security.Claims;
using Application.Operation;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Operations.API.Filters;
using Serilog;
using Serilog.Exceptions;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Infrastructure;
using Tawtheef.Infrastructure.Extensions;
using Tawtheef.Infrastructure.Middlewares;
using EnvironmentName = Tawtheef.Domain.Common.EnvironmentName;

const string myCors = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options => { options.AddServerHeader = false; });

// ----- Configuration -----
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

if (builder.Configuration.GetValue<bool>("KeyVault:Enabled"))
{
    var keyVaultUri = builder.Configuration["KeyVault:Uri"] ??
                      throw new InvalidOperationException("KeyVault:Uri is required when KeyVault:Enabled is true.");
    var clientId = builder.Configuration["KeyVault:ClientId"] ??
                   throw new InvalidOperationException("KeyVault:ClientId is required when KeyVault:Enabled is true.");
    var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions { ManagedIdentityClientId = clientId });
    builder.Configuration.AddAzureKeyVault(new Uri(keyVaultUri), credential, new KeyVaultSecretManager());
}

// ----- Application Insights (SDK) -----
var aiCs =
    builder.Configuration["ApplicationInsights:ConnectionString"]
    ?? builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]
    ?? builder.Configuration["APPSETTING_APPLICATIONINSIGHTS_CONNECTION_STRING"];

var aiEnabled = builder.Configuration.GetValue<bool>("ApplicationInsights:Enabled");

// لو ما بدك flag، احذف الشرط وخليه دايمًا يتفعل لما aiCs موجود
if (aiEnabled && !string.IsNullOrWhiteSpace(aiCs))
{
    builder.Services.AddApplicationInsightsTelemetry(o =>
    {
        o.ConnectionString = aiCs;
        // اختياري:
        // o.EnableAdaptiveSampling = false;
    });
}

// ----- Serilog (single place; reads appsettings.*) -----
builder.Host.UseSerilog((ctx, services, lc) =>
{
    var seqUrl = ctx.Configuration["Seq:Url"];
    var seqKey = ctx.Configuration["Seq:ApiKey"];

    lc.ReadFrom.Configuration(ctx.Configuration)
      .ReadFrom.Services(services)
      .Enrich.FromLogContext()
      .Enrich.WithMachineName()
      .Enrich.WithEnvironmentName()
      .Enrich.WithEnvironmentUserName()
      .Enrich.WithThreadId()
      .Enrich.WithExceptionDetails()
      .Enrich.WithProperty("Application", "Tawtheef.Recruitment")
      .Enrich.WithProperty("Version", "1.0.0")
      .WriteTo.Console(outputTemplate:
          "{Timestamp:HH:mm:ss} [{Level:u3}] ({ThreadId}) {Message:lj}{NewLine}{Exception}")
      .WriteTo.File(
          @"C:\home\LogFiles\app-serilog-tawtheef-.txt",
          rollingInterval: RollingInterval.Year,
          shared: true);

    // Seq
    if (!string.IsNullOrWhiteSpace(seqUrl) && !string.IsNullOrWhiteSpace(seqKey))
        lc.WriteTo.Seq(seqUrl, apiKey: seqKey);

    // Application Insights sink (Serilog -> AI Traces)
    var sinkAiCs =
        ctx.Configuration["ApplicationInsights:ConnectionString"]
        ?? ctx.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]
        ?? ctx.Configuration["APPSETTING_APPLICATIONINSIGHTS_CONNECTION_STRING"];

    if (ctx.Configuration.GetValue<bool>("ApplicationInsights:Enabled") 
        && !string.IsNullOrWhiteSpace(sinkAiCs))
    {
        lc.WriteTo.ApplicationInsights(
            services.GetRequiredService<TelemetryConfiguration>(),
            TelemetryConverter.Traces);
    }
});

// ----- Services -----
builder.Services.Configure<ForwardedHeadersOptions>(o => {
    o.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});

builder.Services.AddInfrastructureLayer(builder.Configuration, builder.Environment);
builder.Services.AddApplicationOperation(builder.Configuration);
builder.Services.AddScoped<AdminActionAuditFilter>();
// builder.Services.AddRecaptcha(builder.Configuration.GetSection("RecaptchaSettings"));

builder.Services.AddControllers(options =>
    {
        options.Filters.AddService<AdminActionAuditFilter>();
    })
    .ConfigureApiBehaviorOptions(options => {
    options.InvalidModelStateResponseFactory = ctx => {
        var problem = new ValidationProblemDetails(ctx.ModelState) {
            Status  = StatusCodes.Status400BadRequest,
            Type    = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
            Title   = "One or more validation errors occurred.",
            Detail  = "See the 'errors' property for details.",
            Instance= ctx.HttpContext.Request.Path,
            Extensions = {
                ["traceId"] = ctx.HttpContext.TraceIdentifier,
                ["correlationId"] = ctx.HttpContext.Items.TryGetValue("CorrelationId", out var cid) ? cid : null
            }
        };

        return new BadRequestObjectResult(problem);
    };
});

builder.Services.AddCors(options => {
    options.AddPolicy(myCors, policy =>
        policy.WithOrigins(builder.Configuration["AppConfig:FrontendUrl"]!)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()
              .WithExposedHeaders("Content-Disposition", "X-Blocked-By"));
});

builder.Services.AddExceptionHandler<CustomExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddHsts(options =>
{
    options.MaxAge = TimeSpan.FromDays(365);
});


if (builder.Environment.EnvironmentName != nameof(EnvironmentName.Production)) {
    builder.Services.AddSwagger();
}

var app = builder.Build();

// ----- Pipeline (order matters) -----
app.UseForwardedHeaders();
app.UseMiddleware<SecurityHeadersMiddleware>();

app.UseLanguageMiddleware();

app.UseMiddleware<RequestSanitizationMiddleware>();
app.UseMiddleware<CorrelationIdMiddleware>();
app.Use(async (ctx, next) =>
{
    var capture = ctx.RequestServices.GetService<IRequestBodyCapture>();
    if (capture != null)
        ctx.Items["RequestBody"] = await capture.TryGetRedactedBodyAsync(ctx);

    await next();
});
app.UseSerilogRequestLogging(opts => {
    opts.EnrichDiagnosticContext = (diagCtx, httpCtx) => {
        var userId = httpCtx.User.FindFirst("sub")?.Value
                     ?? httpCtx.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        diagCtx.Set("CorrelationId", (httpCtx.Items.TryGetValue("CorrelationId", out var cid) ? cid : httpCtx.TraceIdentifier) ?? "");
        diagCtx.Set("UserId", userId ?? "anonymous");
        diagCtx.Set("QueryString", httpCtx.Request.QueryString.HasValue ? httpCtx.Request.QueryString.Value : "");
        diagCtx.Set("Route", httpCtx.GetEndpoint()?.DisplayName ?? "");
        diagCtx.Set("ClientIP", httpCtx.Connection.RemoteIpAddress?.ToString() ?? "unknown");
        diagCtx.Set("Path", httpCtx.Request.Path);
        var capture = httpCtx.RequestServices.GetService<IRequestBodyCapture>();
        if (capture is null)
            return;

        diagCtx.Set("RequestBody", httpCtx.Items.TryGetValue("RequestBody", out var v) ? v : "");
    };
});

app.UseMiddleware<ResponseLoggingMiddleware>();

app.UseExceptionHandler(appBuilder =>
{
    appBuilder.Run(async context =>
    {
        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        if (exceptionHandlerPathFeature?.Error != null)
        {
            var handler = context.RequestServices.GetRequiredService<CustomExceptionHandler>();
            var handled = await handler.TryHandleAsync(context, exceptionHandlerPathFeature.Error, CancellationToken.None);
            if (!handled)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            }
        }
    });
});
app.UseHsts();

if (builder.Environment.EnvironmentName != nameof(EnvironmentName.Production))
    app.MapSwagger();

app.UseHttpsRedirection();

app.UseCors(myCors);
app.UseCookiePolicy(); 
app.UseAuthentication();
app.UseAuthorization();

//enable rate limiter middleware
app.UseRateLimiter();

app.MapGet("/", () => Results.Json(new { status = "" }));
app.MapControllers();
app.Run();
