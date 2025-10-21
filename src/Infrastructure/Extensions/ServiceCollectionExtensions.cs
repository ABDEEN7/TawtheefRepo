using System;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.RateLimiting;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using Microsoft.IdentityModel.Tokens;
using Tawtheef.Application.Common.Interfaces.NotificationServices;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Domain.Configurations;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Infrastructure.Data;
using Tawtheef.Infrastructure.Data.Interceptors;
using Tawtheef.Infrastructure.Repositories;
using Tawtheef.Infrastructure.Repositories.Base;
using Tawtheef.Infrastructure.Services;
using Tawtheef.Infrastructure.Services.NotificationServices;
using Tawtheef.Infrastructure.Services.StorageServices;

namespace Tawtheef.Infrastructure.Extensions;
public static class LimitsPolicyKeys
{
    public const string VerificationPolicy = "VerificationPolicy";
    public const string ApplyDiscountCodePolicy = "ApplyDiscountCodePolicy";
    public const string ContactUsEmailPolicy = "ContactUsEmailPolicy";
}
public static class ServiceCollectionExtensions
{    
    public static void AddInfrastructureLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddFeatureManagement();
        
        services.Configure<AppConfigSettings>(configuration.GetSection("AppConfig"));
        services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
        services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
        services.Configure<RecaptchaSettings>(configuration.GetSection("RecaptchaSettings"));
        services.Configure<StorageSettings>(configuration.GetSection("Storage"));
        services.Configure<EmailDispatcherSettings>(configuration.GetSection("EmailDispatcher"));
        services.AddTransient<IMediator, Mediator>();
        
        services.AddNotificationServices();
        services.AddServices(configuration);
        
        services.AddDbContext(configuration);
        services.AddRepositories();
        services.AddAuthorization(configuration);
    }
    
    /// <summary>
    /// Add DbContext with SQL Server for Debug and MySQL for Release
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    private static void AddDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        var cs = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<TawtheefDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetRequiredService<AuditableEntityInterceptor>());
        });
        services.AddIdentity<User, IdentityRole<Guid>>(options => {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
            })
            .AddEntityFrameworkStores<TawtheefDbContext>()
            .AddDefaultTokenProviders();
    }

    private static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<AuditableEntityInterceptor>();
        
        services
            .AddTransient(typeof(IUnitOfWork), typeof(UnitOfWork))
            .AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>))
            .AddTransient<IUserRepository, UserRepository>();
    }

    private static void AddAuthorization(this IServiceCollection services, IConfiguration configuration)
{
    // Azure AD settings (appsettings.json)
    // "AzureAd": {
    //   "Instance": "https://login.microsoftonline.com/",
    //   "TenantId": "<TENANT_ID>",
    //   "ClientId": "<API_CLIENT_ID_OR_AUDIENCE>",
    //   "CallbackPath": "/signin-oidc"  // optional for server-side interactive login
    // }

    var azureInstance = configuration["AzureAd:Instance"];
    var azureTenantId = configuration["AzureAd:TenantId"];
    var azureClientId = configuration["AzureAd:ClientId"];
    var azureAuthority = !string.IsNullOrEmpty(azureTenantId) && !string.IsNullOrEmpty(azureInstance)
        ? $"{azureInstance.TrimEnd('/')}/{azureTenantId}/v2.0"
        : null;

    services.AddAuthentication(options =>
        {
            options.DefaultScheme = "Smart";
            options.DefaultAuthenticateScheme = "Smart";
            options.DefaultChallengeScheme = "Smart";
        })
        .AddPolicyScheme("Smart", "JWT or Cookies", opt =>
        {
            opt.ForwardDefaultSelector = ctx =>
                ctx.Request.Headers.ContainsKey("Authorization") &&
                ctx.Request.Headers.Authorization.ToString().StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                    ? JwtBearerDefaults.AuthenticationScheme
                    : IdentityConstants.ApplicationScheme;
        })
        // JWT Bearer (for tokens issued by your own server OR Azure AD tokens)
        .AddJwtBearer(options =>
        {
            // If using Azure AD tokens, set Authority and Audience accordingly.
            if (!string.IsNullOrEmpty(azureAuthority) && !string.IsNullOrEmpty(azureClientId))
            {
                options.Authority = azureAuthority;
                options.Audience = azureClientId; // sometimes use "api://{clientId}" depending on app registration
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidIssuer = null, // let middleware validate against metadata (issuer validation enabled)
                    ValidateAudience = true,
                    ValidAudience = configuration["Jwt:Audience"] ?? azureClientId,
                    RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
                };
            }
            else
            {
                // fallback to your existing symmetric-key JWT validation (local tokens)
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!)),
                    ValidateIssuer = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = configuration["Jwt:Audience"],
                    RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
                };
            }

            // 👇 Prevents 302 redirect and returns 401/403 (keeps your previous behavior)
            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = ctx =>
                {
                    Console.WriteLine($"Auth failed: {ctx.Exception}");
                    return Task.CompletedTask;
                },
                OnChallenge = async context =>
                {
                    Console.WriteLine($"JWT Challenge: {context.Error}");
                    context.HandleResponse(); // Skip default redirect behavior
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(JsonSerializer.Serialize(new
                    {
                        StatusCode = 401,
                        Message = "Unauthorized: Invalid or expired token."
                    }));
                },
                OnForbidden = async context =>
                {
                    Console.WriteLine($"JWT Failed: {context.Result}");
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(JsonSerializer.Serialize(new
                    {
                        StatusCode = 403,
                        Message = "Forbidden: You don't have permission."
                    }));
                }
            };
        })
        .AddOpenIdConnect("AzureAD", options =>
        {
            if (string.IsNullOrEmpty(azureAuthority) || string.IsNullOrEmpty(azureClientId))
                throw new InvalidOperationException("AzureAd configuration missing.");

            options.Authority = azureAuthority;
            options.ClientId = azureClientId;
            options.CallbackPath = configuration["AzureAd:CallbackPath"] ?? "/signin-oidc";
            options.ResponseType = "code";
            options.SaveTokens = true;
            options.SignInScheme = IdentityConstants.ApplicationScheme; // sign in to Identity cookie
            options.TokenValidationParameters = new TokenValidationParameters
            {
                RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
            };

            // IMPORTANT: Prevent redirect loops for API requests (return 401 JSON instead of 302).
            options.Events = new OpenIdConnectEvents
            {
                OnRedirectToIdentityProvider = ctx =>
                {
                    if (IsApiRequest(ctx.Request))
                    {
                        ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        ctx.Response.ContentType = "application/json";
                        ctx.HandleResponse();
                        var payload = JsonSerializer.Serialize(new { StatusCode = 401, Message = "Unauthorized: interactive login required." });
                        return ctx.Response.WriteAsync(payload);
                    }
                    return Task.CompletedTask;
                },
                OnRedirectToIdentityProviderForSignOut = ctx =>
                {
                    if (IsApiRequest(ctx.Request))
                    {
                        ctx.Response.StatusCode = StatusCodes.Status200OK;
                        ctx.HandleResponse();
                    }
                    return Task.CompletedTask;
                }
            };
        })
        // Keep existing Google & Microsoft personal accounts (optional)
        .AddGoogle(o =>
        {
            o.ClientId = configuration["Authentication:Google:ClientId"]!;
            o.ClientSecret = configuration["Authentication:Google:ClientSecret"]!;
            o.SignInScheme = IdentityConstants.ExternalScheme;
        })
        .AddMicrosoftAccount(microsoftOptions =>
        {
            microsoftOptions.ClientId = configuration["Authentication:Microsoft:ClientId"]!;
            microsoftOptions.ClientSecret = configuration["Authentication:Microsoft:ClientSecret"]!;
            microsoftOptions.SignInScheme = IdentityConstants.ExternalScheme;
        });

    // Rate limiter / Policies — keep as-is
    services.AddRateLimiter(options =>
    {
        options.AddPolicy(LimitsPolicyKeys.VerificationPolicy, context =>
        {
            var email = context.Request.Query["email"].ToString();
            return RateLimitPartition.GetSlidingWindowLimiter(
                partitionKey: email,
                factory: _ => new SlidingWindowRateLimiterOptions
                {
                    PermitLimit = 3,
                    Window = TimeSpan.FromMinutes(10),
                    SegmentsPerWindow = 2
                });
        });

        options.AddPolicy(LimitsPolicyKeys.ApplyDiscountCodePolicy, context =>
        {
            var studentId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(studentId))
                return RateLimitPartition.GetSlidingWindowLimiter(
                    partitionKey: "anonymous",
                    factory: _ => new SlidingWindowRateLimiterOptions
                    {
                        PermitLimit = 2,
                        Window = TimeSpan.FromMinutes(15),
                        SegmentsPerWindow = 3
                    });

            return RateLimitPartition.GetSlidingWindowLimiter(
                partitionKey: studentId,
                factory: _ => new SlidingWindowRateLimiterOptions
                {
                    PermitLimit = 5,
                    Window = TimeSpan.FromMinutes(15),
                    SegmentsPerWindow = 3
                });
        });

        options.AddFixedWindowLimiter(LimitsPolicyKeys.ContactUsEmailPolicy, opt =>
        {
            opt.Window = TimeSpan.FromMinutes(1);
            opt.PermitLimit = 5; // Max 5 requests per minute
        });
    });

    services.AddAuthorizationBuilder()
            .AddPolicy(nameof(UserTypeIds.Admin), policy => policy.RequireRole(nameof(UserTypeIds.Admin)))
            .AddPolicy(nameof(UserTypeIds.Employee), policy => policy.RequireRole(nameof(UserTypeIds.Employee)))
            .AddPolicy(nameof(UserTypeIds.Applicant), policy => policy.RequireRole(nameof(UserTypeIds.Applicant)));
}

// Helper to consider a request as API (so we don't redirect)
// tweak rules as you need (path prefix, Accept header, or X-Requested-With)
private static bool IsApiRequest(HttpRequest request)
{
    // treat anything under /api as API
    if (request.Path.StartsWithSegments("/api", StringComparison.OrdinalIgnoreCase)) return true;

    // or if Accept header expects json
    if (request.Headers.TryGetValue("Accept", out var accept))
    {
        if (accept.ToString().Contains("application/json", StringComparison.OrdinalIgnoreCase)) return true;
    }

    // or X-Requested-With (AJAX)
    if (request.Headers.TryGetValue("X-Requested-With", out var xrw) && xrw == "XMLHttpRequest") return true;

    return false;
}



    private static void AddNotificationServices(this IServiceCollection services)
    {
        services.AddSingleton<IEmailBranding, DefaultBranding>();
        services.AddSingleton<IEmailQueue, EmailQueue>();
        services.AddSingleton<IEmailTransport, MailKitEmailTransport>();
        services.AddSingleton<IEmailTemplateRenderer, RazorTemplateRenderer>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddHostedService<EmailDispatcher>();
    }
    private static void AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration["Storage:ConnectionString"];
        var containerName  = configuration["Storage:RootPath"]!;

        services.AddSingleton(sp => new BlobServiceClient(connectionString));
        services.AddScoped(sp =>
        {
            var serviceClient = sp.GetRequiredService<BlobServiceClient>();
            var containerClient = serviceClient.GetBlobContainerClient(containerName);
            return containerClient;
        });
        
        services.AddScoped<IFileStorageService, AzureBlobStorageService>()
                .AddScoped<IPasswordVerifier, PasswordVerifier>();

        services.AddScoped<ITokenService, TokenService>()
            .AddScoped<IOtpService, OtpService>()
            .AddScoped<IVerificationService, VerificationService>()
            .AddScoped<ICurrentUserService, CurrentUserService>();

        services.AddScoped<IMediaUrlResolver, MediaUrlResolver>();

        
        services.AddHttpClient<IRecaptchaService, RecaptchaService>();
        
        services.AddSingleton<IEnvironmentNameProvider, EnvironmentNameProvider>();
    }
}
