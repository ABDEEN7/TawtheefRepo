using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.RateLimiting;
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

namespace Tawtheef.Infrastructure
{
    /// <summary>
    /// Keys for rate limiting policies used across the application.
    /// </summary>
    public static class LimitsPolicyKeys
    {
        public const string VerificationPolicy = "VerificationPolicy";
        public const string ApplyDiscountCodePolicy = "ApplyDiscountCodePolicy";
        public const string ContactUsEmailPolicy = "ContactUsEmailPolicy";
    }

    /// <summary>
    /// Extension methods for registering infrastructure layer services.
    /// </summary>
    public static class DependencyInjectionExtensions
    {
        /// <summary>
        /// Registers all infrastructure services (DB, repos, auth, notifications, storage, etc).
        /// </summary>
        public static void AddInfrastructureLayer(this IServiceCollection services, IConfiguration configuration)
        {
            // Feature flags
            services.AddFeatureManagement();

            // Configuration objects (IOptions<T>)
            ConfigureOptions(services, configuration);

            // Core frameworks & mediator
            services.AddTransient<IMediator, Mediator>();

            // App-specific services
            services.AddNotificationServices();
            services.AddServices(configuration);

            // Data & Repositories
            services.AddDbContext(configuration);
            services.AddRepositories();

            // Authentication & Authorization
            services.AddAuthorizationAndAuthentication(configuration);
        }

        #region Configuration Helpers

        private static void ConfigureOptions(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AppConfigSettings>(configuration.GetSection("AppConfig"));
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
            services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
            services.Configure<RecaptchaSettings>(configuration.GetSection("RecaptchaSettings"));
            services.Configure<StorageSettings>(configuration.GetSection("Storage"));
            services.Configure<EmailDispatcherSettings>(configuration.GetSection("EmailDispatcher"));
        }

        #endregion

        #region DbContext & Identity

        /// <summary>
        /// Adds the TawtheefDbContext and Identity configuration.
        /// </summary>
        private static void AddDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            // Connection string kept for potential conditional logic later
            var cs = configuration.GetConnectionString("DefaultConnection");

            services.AddScoped<AuditableEntityInterceptor>();

            services.AddDbContext<TawtheefDbContext>((sp, options) =>
            {
                // register interceptors or other options as needed
                options.AddInterceptors(sp.GetRequiredService<AuditableEntityInterceptor>());
            });

            services.AddIdentity<User, IdentityRole<Guid>>(options =>
            {
                // password policy
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
            })
            .AddEntityFrameworkStores<TawtheefDbContext>()
            .AddDefaultTokenProviders();
        }

        #endregion

        #region Repositories

        private static void AddRepositories(this IServiceCollection services)
        {
            services
                .AddTransient<IUnitOfWork, UnitOfWork>()
                .AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>))
                .AddTransient<IUserRepository, UserRepository>();
        }

        #endregion

        #region Authentication & Authorization

        /// <summary>
        /// Registers authentication schemes (JWT, Azure AD OIDC fallback, external providers) and authorization policies.
        /// </summary>
        private static void AddAuthorizationAndAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            // Read Azure AD settings
            var azureInstance = configuration["AzureAd:Instance"];
            var azureTenantId = configuration["AzureAd:TenantId"];
            var azureClientId = configuration["AzureAd:ClientId"];
            var azureAuthority = !string.IsNullOrEmpty(azureTenantId) && !string.IsNullOrEmpty(azureInstance)
                ? $"{azureInstance.TrimEnd('/')}/{azureTenantId}/v2.0"
                : null;

            ConfigureAuthentication(services, configuration, azureAuthority, azureClientId);
            ConfigureRateLimitingPolicies(services);
            ConfigureAuthorizationPolicies(services);
        }

        private static void ConfigureAuthentication(IServiceCollection services, IConfiguration configuration, string? azureAuthority, string? azureClientId)
        {
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
            .AddJwtBearer(options =>
            {
                // Azure AD issued tokens case
                if (!string.IsNullOrEmpty(azureAuthority) && !string.IsNullOrEmpty(azureClientId))
                {
                    options.Authority = azureAuthority;
                    options.Audience = azureClientId;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = true,
                        // Allow middleware to validate via metadata
                        ValidAudience = configuration["Jwt:Audience"] ?? azureClientId,
                        ValidateAudience = true,
                        RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
                    };
                }
                else
                {
                    // Local symmetric-key tokens
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"] ?? string.Empty)),
                        ValidateIssuer = true,
                        ValidIssuer = configuration["Jwt:Issuer"],
                        ValidateAudience = true,
                        ValidAudience = configuration["Jwt:Audience"],
                        RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
                    };
                }

                // Prevent redirects -> return JSON 401/403 for API clients
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
                        context.HandleResponse();
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
                        Console.WriteLine($"JWT Forbidden: {context.Result}");
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
                options.SignInScheme = IdentityConstants.ApplicationScheme;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
                };

                // Return JSON 401 for API requests instead of redirecting
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
            .AddGoogle(o =>
            {
                o.ClientId = configuration["Authentication:Google:ClientId"] ?? string.Empty;
                o.ClientSecret = configuration["Authentication:Google:ClientSecret"] ?? string.Empty;
                o.SignInScheme = IdentityConstants.ExternalScheme;
            })
            .AddMicrosoftAccount(microsoftOptions =>
            {
                microsoftOptions.ClientId = configuration["Authentication:Microsoft:ClientId"] ?? string.Empty;
                microsoftOptions.ClientSecret = configuration["Authentication:Microsoft:ClientSecret"] ?? string.Empty;
                microsoftOptions.SignInScheme = IdentityConstants.ExternalScheme;
            });
        }

        /// <summary>
        /// Configure rate limiting policies used by the application.
        /// </summary>
        private static void ConfigureRateLimitingPolicies(IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                // Verification policy (per email)
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

                // Apply discount code policy (per user or anonymous)
                options.AddPolicy(LimitsPolicyKeys.ApplyDiscountCodePolicy, context =>
                {
                    var studentId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (string.IsNullOrEmpty(studentId))
                    {
                        return RateLimitPartition.GetSlidingWindowLimiter(
                            partitionKey: "anonymous",
                            factory: _ => new SlidingWindowRateLimiterOptions
                            {
                                PermitLimit = 2,
                                Window = TimeSpan.FromMinutes(15),
                                SegmentsPerWindow = 3
                            });
                    }

                    return RateLimitPartition.GetSlidingWindowLimiter(
                        partitionKey: studentId,
                        factory: _ => new SlidingWindowRateLimiterOptions
                        {
                            PermitLimit = 5,
                            Window = TimeSpan.FromMinutes(15),
                            SegmentsPerWindow = 3
                        });
                });

                // Contact us (fixed window)
                options.AddFixedWindowLimiter(LimitsPolicyKeys.ContactUsEmailPolicy, opt =>
                {
                    opt.Window = TimeSpan.FromMinutes(1);
                    opt.PermitLimit = 5;
                });
            });
        }

        private static void ConfigureAuthorizationPolicies(IServiceCollection services)
        {
            services.AddAuthorizationBuilder()
                    .AddPolicy(nameof(UserTypeIds.Admin), policy => policy.RequireRole(nameof(UserTypeIds.Admin)))
                    .AddPolicy(nameof(UserTypeIds.Employee), policy => policy.RequireRole(nameof(UserTypeIds.Employee)))
                    .AddPolicy(nameof(UserTypeIds.Applicant), policy => policy.RequireRole(nameof(UserTypeIds.Applicant)));
        }

        /// <summary>
        /// Helper to determine whether the request is an API call (used to prevent interactive redirects).
        /// </summary>
        private static bool IsApiRequest(HttpRequest request)
        {
            if (request.Path.StartsWithSegments("/api", StringComparison.OrdinalIgnoreCase))
                return true;

            if (request.Headers.TryGetValue("Accept", out var accept) &&
                accept.ToString().Contains("application/json", StringComparison.OrdinalIgnoreCase))
                return true;

            if (request.Headers.TryGetValue("X-Requested-With", out var xrw) && xrw == "XMLHttpRequest")
                return true;

            return false;
        }

        #endregion

        #region Notification Services & App Services

        /// <summary>
        /// Registers notification-related services (email queue, templates, dispatcher...).
        /// </summary>
        private static void AddNotificationServices(this IServiceCollection services)
        {
            services.AddSingleton<IEmailBranding, DefaultBranding>();
            services.AddSingleton<IEmailQueue, EmailQueue>();
            services.AddSingleton<IEmailTransport, MailKitEmailTransport>();
            services.AddSingleton<IEmailTemplateRenderer, RazorTemplateRenderer>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddHostedService<EmailDispatcher>();
        }

        /// <summary>
        /// Registers general application services (storage clients, token services, etc).
        /// </summary>
        private static void AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration["Storage:ConnectionString"] ?? string.Empty;
            var containerName = configuration["Storage:RootPath"] ?? string.Empty;

            services.AddSingleton(sp => new BlobServiceClient(connectionString));
            services.AddScoped(sp =>
            {
                var serviceClient = sp.GetRequiredService<BlobServiceClient>();
                var containerClient = serviceClient.GetBlobContainerClient(containerName);
                return containerClient;
            });

            services.AddScoped<IFileStorageService, AzureBlobStorageService>();
            services.AddScoped<IPasswordVerifier, PasswordVerifier>();

            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IOtpService, OtpService>();
            services.AddScoped<IVerificationService, VerificationService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            services.AddScoped<IMediaUrlResolver, MediaUrlResolver>();

            services.AddHttpClient<IRecaptchaService, RecaptchaService>();

            services.AddSingleton<IEnvironmentNameProvider, EnvironmentNameProvider>();
            
            services.AddSingleton<ILocalizationService, LocalizationService>();
        }

        #endregion
    }
}
