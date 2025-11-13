using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using reCAPTCHA.AspNetCore;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Tawtheef.Application;
using Tawtheef.Application.Common.Constants;
using Tawtheef.Application.Extensions;
using Tawtheef.Infrastructure;
using Tawtheef.Infrastructure.Extensions;
using Tawtheef.Infrastructure.Middlewares;
using Tawtheef.Infrastructure.Services;

const string myCors = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);

// ----- Configuration -----
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// ----- Serilog + Seq (single place; reads appsettings.*) -----
builder.Host.UseSerilog((ctx, services, lc) => lc
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithExceptionDetails()
    .ReadFrom.Configuration(ctx.Configuration) // uses Seq:Url and Seq:ApiKey
    .ReadFrom.Services(services)
    .WriteTo.Console()
    .WriteTo.Seq(
        serverUrl: ctx.Configuration["Seq:Url"] ?? "http://127.0.0.1:5341",
        apiKey: ctx.Configuration["Seq:ApiKey"])
);

// ----- Services -----
builder.Services.AddInfrastructureLayer(builder.Configuration);
builder.Services.AddApplicationLayer();
builder.Services.AddRecaptcha(builder.Configuration.GetSection("RecaptchaSettings"));
builder.Services.AddSingleton<IAuthorizationHandler, ProfileCompletedHandler>();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(AuthPolicyNames.CompletedProfile, 
        policy => policy.Requirements.Add(new ProfileCompletedRequirement()));
});
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options => {
    options.InvalidModelStateResponseFactory = ctx =>
    {
        var problem = new ValidationProblemDetails(ctx.ModelState)
        {
            Status  = StatusCodes.Status400BadRequest,
            Type    = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
            Title   = "One or more validation errors occurred.",
            Detail  = "See the 'errors' property for details.",
            Instance= ctx.HttpContext.Request.Path,
            Extensions =
            {
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

// Reverse-proxy awareness (nginx)
builder.Services.Configure<ForwardedHeadersOptions>(o =>
{
    o.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});

var app = builder.Build();

// ----- Pipeline (order matters) -----
app.UseForwardedHeaders();

app.UseLanguageMiddleware();

app.UseMiddleware<CorrelationIdMiddleware>();

app.UseSerilogRequestLogging(opts =>
{
    opts.EnrichDiagnosticContext = (diag, http) =>
    {
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
app.UseMiddleware<ResponseLoggingMiddleware>(); 

app.UseCors(myCors);
app.UseCookiePolicy(); 
app.UseAuthentication();
app.UseAuthorization();


app.MapGet("/", () => Results.Json(new { status = "" }));
app.MapControllers();
app.Run();
