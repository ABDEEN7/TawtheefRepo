using System.Security.Claims;
using Application.Recruitment;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Sinks.ApplicationInsights.TelemetryConverters;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Common.Security;
using Tawtheef.Infrastructure;
using Tawtheef.Infrastructure.Extensions;
using Tawtheef.Infrastructure.Middlewares;
using Tawtheef.Infrastructure.Services.Authorization;

const string myCors = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);

// ----- Configuration -----
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

if (builder.Configuration.GetValue<bool>("KeyVault:Enabled")) {
    var keyVaultUri = builder.Configuration["KeyVault:Uri"] ??
                      throw new InvalidOperationException("KeyVault:Uri is required when KeyVault:Enabled is true.");
    var clientId = builder.Configuration["KeyVault:ClientId"] ??
                   throw new InvalidOperationException("KeyVault:ClientId is required when KeyVault:Enabled is true.");
    var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions { ManagedIdentityClientId = clientId });
    builder.Configuration.AddAzureKeyVault(new Uri(keyVaultUri), credential, new KeyVaultSecretManager());
}

builder.Configuration.AddEnvironmentVariables();

// ----- Serilog + Seq (single place; reads appsettings.*) -----
if (builder.Configuration.GetValue<bool>("AzureMonitor:Enabled")) {
    var aiCs = builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"];
    Console.WriteLine($"AI CS exists: {!string.IsNullOrWhiteSpace(aiCs)}; length={(aiCs?.Length ?? 0)}");
    builder.Services.AddOpenTelemetry().UseAzureMonitor();
    builder.Services.AddSingleton(_ => {
        var cfg = TelemetryConfiguration.CreateDefault();
        cfg.ConnectionString = aiCs;
        return cfg;
    });
}

builder.Host.UseSerilog((ctx, services, lc) => {
    var seqUrl = ctx.Configuration["Seq:Url"];
    var seqKey = ctx.Configuration["Seq:ApiKey"];
    lc.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
        .Enrich.FromLogContext()
        .Enrich.WithMachineName()
        .Enrich.WithExceptionDetails()
        .ReadFrom.Configuration(ctx.Configuration)
        .ReadFrom.Services(services)
        .WriteTo.Console();

    // Seq
    if (!string.IsNullOrWhiteSpace(seqUrl) && !string.IsNullOrWhiteSpace(seqKey))
        lc.WriteTo.Seq(seqUrl, apiKey: seqKey);

    // Application Insights
    if (builder.Configuration.GetValue<bool>("AzureMonitor:Enabled"))
        lc.WriteTo.ApplicationInsights(
            services.GetRequiredService<TelemetryConfiguration>(),
            new TraceTelemetryConverter());
});

// ----- Services -----
builder.Services.Configure<ForwardedHeadersOptions>(o => {
    o.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});

builder.Services.AddInfrastructureLayer(builder.Configuration, builder.Environment);
builder.Services.AddApplicationRecruitment(builder.Configuration);
// builder.Services.AddRecaptcha(builder.Configuration.GetSection("RecaptchaSettings"));
builder.Services.AddAuthorization(options => {
    options.AddPolicy(PolicyNames.CompletedProfile,
        policy => policy.Requirements.Add(new ProfileCompletedRequirement()));
});

builder.Services.AddControllers()
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
              .WithExposedHeaders("Content-Disposition"));
});

builder.Services.AddExceptionHandler<CustomExceptionHandler>();
builder.Services.AddProblemDetails();

#if DEBUG
builder.Services.AddSwagger();
#endif

var app = builder.Build();

// ----- Pipeline (order matters) -----
app.UseForwardedHeaders();

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

#if DEBUG
app.UseDeveloperExceptionPage();
app.MapSwagger();
#else
    app.UseExceptionHandler();
#endif
app.UseMiddleware<ResponseLoggingMiddleware>();

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
