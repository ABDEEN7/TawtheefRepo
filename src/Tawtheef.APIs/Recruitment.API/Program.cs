using System.Security.Claims;
using Application.Recruitment;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Tawtheef.Application;
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
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

if (builder.Configuration.GetValue<bool>("KeyVault:Enabled")) {
    var keyVaultUri = builder.Configuration["KeyVault:Uri"] ??
                      throw new InvalidOperationException("KeyVault:Uri is required when KeyVault:Enabled is true.");

    builder.Configuration.AddAzureKeyVault(
        new Uri(keyVaultUri),
        new DefaultAzureCredential(),
        new KeyVaultSecretManager());
    
    // Enable OpenTelemetry -> Azure Monitor (Application Insights)
    builder.Services.AddOpenTelemetry().UseAzureMonitor();
}
// ----- Serilog + Seq (single place; reads appsettings.*) -----
builder.Host.UseSerilog((ctx, services, lc) => {
        var seqUrl = ctx.Configuration["Seq:Url"];
        var seqKey = ctx.Configuration["Seq:ApiKey"];
        var config = lc.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithExceptionDetails()
            .ReadFrom.Configuration(ctx.Configuration)
            .ReadFrom.Services(services)
            .WriteTo.Console();
            
        if(!string.IsNullOrWhiteSpace(seqUrl) && !string.IsNullOrWhiteSpace(seqKey))
            config.WriteTo.Seq(serverUrl: seqUrl,apiKey: seqKey);
    }
);

// ----- Services -----
builder.Services.Configure<ForwardedHeadersOptions>(o => {
    o.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});

builder.Services.AddInfrastructureLayer(builder.Configuration, builder.Environment);
builder.Services.AddApplicationLayer(builder.Configuration);
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

app.UseSerilogRequestLogging(opts => {
    opts.EnrichDiagnosticContext = (diag, http) => {
        var userId = http.User.FindFirst("sub")?.Value
                  ?? http.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        diag.Set("CorrelationId", (http.Items.TryGetValue("CorrelationId", out var cid) ? cid : http.TraceIdentifier) ?? "");
        diag.Set("UserId", userId ?? "anonymous");
        diag.Set("QueryString", http.Request.QueryString.HasValue ? http.Request.QueryString.Value : "");
        diag.Set("Route", http.GetEndpoint()?.DisplayName ?? "");
        diag.Set("ClientIP", http.Connection.RemoteIpAddress?.ToString() ?? "unknown");
        diag.Set("Path", http.Request.Path);
    };
});

#if DEBUG
app.UseDeveloperExceptionPage();
app.MapSwagger();
app.UseHttpsRedirection();
#else
    app.UseExceptionHandler();
#endif
app.UseExceptionHandlingMiddleware();
app.UseMiddleware<ResponseLoggingMiddleware>(); 
app.Use(async (context, next) => {
    context.Request.Scheme = "https";
    await next();
});
app.UseForwardedHeaders();

app.UseCors(myCors);
app.UseCookiePolicy(); 
app.UseAuthentication();
app.UseAuthorization();

//enable rate limiter middleware
app.UseRateLimiter();

app.MapGet("/", () => Results.Json(new { status = "" }));
app.MapControllers();
app.Run();
