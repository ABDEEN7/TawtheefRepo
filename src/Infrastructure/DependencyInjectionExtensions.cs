using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Threading.RateLimiting;
using Azure.Storage.Blobs;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Tawtheef.Application.Common.Constants;
using Tawtheef.Application.Common.Interfaces;
using Tawtheef.Application.Common.Interfaces.NotificationServices;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Common.Interfaces.Services.HttpClients;
using Tawtheef.Domain.Configurations.Settings;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Infrastructure.Data;
using Tawtheef.Infrastructure.Data.Interceptors;
using Tawtheef.Infrastructure.Repositories;
using Tawtheef.Infrastructure.Repositories.Base;
using Tawtheef.Infrastructure.Services;
using Tawtheef.Infrastructure.Services.HttpClients;
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
        const string Smart = "Smart";
        const string AzureOidc = "AzureOIDC";
        const string AppCookieName = ".tawtheef.auth";
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
            
            RegisterHttpClients(services, configuration);
        }

        #region Configuration Helpers

        private static void ConfigureOptions(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtSettings>(configuration.GetSection("Authentication:Jwt"));
            services.Configure<AppConfigSettings>(configuration.GetSection("AppConfig"));
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
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
                options.UseSqlServer(cs, sql =>
                        {
                            sql.MigrationsAssembly(typeof(TawtheefDbContext).Assembly.FullName);
                            sql.EnableRetryOnFailure(5);
                        })
                        .AddInterceptors(sp.GetRequiredService<AuditableEntityInterceptor>());
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
            ConfigureAuthentication(services, configuration);
            ConfigureRateLimitingPolicies(services);
            ConfigureAuthorizationPolicies(services);
        }
        private static void RegisterHttpClients(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<QatarPassAuthSettings>(configuration.GetSection(QatarPassAuthSettings.SectionName));
            services.AddHttpClient<IQatarPassClient, QatarPassClient>();
            services.Configure<HodhodSmsSettings>(configuration.GetSection(HodhodSmsSettings.SectionName));
            services.AddHttpClient<ISmsGatewayClient, HodhodSmsClient>();
        }

        private static void ConfigureAuthentication(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<CookiePolicyOptions>(options => {
                options.MinimumSameSitePolicy = SameSiteMode.None;
                options.OnAppendCookie = ctx =>
                {
                    if (ctx.CookieOptions.SameSite == SameSiteMode.Lax)
                        ctx.CookieOptions.SameSite = SameSiteMode.None;
                    ctx.CookieOptions.Secure = true;
                };
                options.OnDeleteCookie = ctx =>
                {
                    if (ctx.CookieOptions.SameSite == SameSiteMode.Lax)
                        ctx.CookieOptions.SameSite = SameSiteMode.None;
                    ctx.CookieOptions.Secure = true;
                };
            });

            var jwtSection = configuration.GetSection("Authentication:Jwt");
            var issuer = jwtSection["Issuer"]!;
            var audience = jwtSection["Audience"]!;
            var signingKeyRaw = jwtSection["SigningKey"]!;
            
            var signingKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(signingKeyRaw));

            services.AddAuthentication(options => {
                    options.DefaultScheme = Smart;
                    options.DefaultAuthenticateScheme = Smart;
                    options.DefaultChallengeScheme = Smart;
                })
                .AddPolicyScheme(Smart, "Cookie or Bearer", o => {
                    o.ForwardDefaultSelector = ctx =>
                        ctx.Request.Headers.TryGetValue("Authorization", out var h) &&
                        h.ToString().StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                            ? JwtBearerDefaults.AuthenticationScheme
                            : CookieAuthenticationDefaults.AuthenticationScheme;
                })
                .AddCookie(o => {
                    o.Cookie.Name = AppCookieName;
                    o.Cookie.SameSite = SameSiteMode.None;
                    o.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    o.SlidingExpiration = true;
                })
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options => {
                    options.RequireHttpsMetadata = true;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = issuer,
                        ValidateAudience = true,
                        ValidAudience = audience,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = signingKey,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromMinutes(1)
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = async ctx =>
                        {
                            var sidFromToken = ctx.Principal?.FindFirst("sid")?.Value
                                               ?? ctx.Principal?.FindFirst(JwtRegisteredClaimNames.Sid)?.Value;

                            var userId = ctx.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                            if (string.IsNullOrEmpty(sidFromToken) || string.IsNullOrEmpty(userId))
                            {
                                ctx.Fail("Missing sid or user id.");
                                return;
                            }

                            var sessionService = ctx.HttpContext.RequestServices.GetRequiredService<ISessionService>();
                            var currentSid = await sessionService.GetCurrentAsync(Guid.Parse(userId), ctx.HttpContext.RequestAborted);

                            if (string.IsNullOrEmpty(currentSid) || !string.Equals(currentSid, sidFromToken, StringComparison.Ordinal))
                            {
                                ctx.Fail("Session changed. Please sign in again.");
                            }
                        }
                    };
                });

            var google = configuration.GetSection("Authentication:Google");
            var googleClientId = google["ClientId"];
            var googleClientSecret = google["ClientSecret"];
            var googleEnabled = !string.IsNullOrWhiteSpace(googleClientId) &&
                                !string.IsNullOrWhiteSpace(googleClientSecret);

            if (googleEnabled)
            {
                services.AddAuthentication().AddGoogle(options => {
                    options.ClientId = googleClientId!;
                    options.ClientSecret = googleClientSecret!;
                    options.SignInScheme = IdentityConstants.ExternalScheme;
                    options.SaveTokens = true;
                    options.Scope.Add("email");
                    options.Scope.Add("profile");
                });
            }

            var azure = configuration.GetSection("Authentication:Azure");
            var azureClientId = azure["ClientId"];
            var azureTenantId = azure["TenantId"];
            var azureInstance = azure["Instance"];

            var azureEnabled =
                !string.IsNullOrWhiteSpace(azureClientId) &&
                !string.IsNullOrWhiteSpace(azureTenantId) &&
                !string.IsNullOrWhiteSpace(azureInstance);

            if (azureEnabled)
            {
                services.AddAuthentication()
                    .AddMicrosoftIdentityWebApp(configuration, "Authentication:Azure",
                        openIdConnectScheme: AzureOidc);

                services.PostConfigure<OpenIdConnectOptions>(AzureOidc, o => {
                    o.SignInScheme = IdentityConstants.ExternalScheme;
                    o.ResponseType = OpenIdConnectResponseType.Code;
                    o.SaveTokens = true;
                    o.Scope.Add("openid");
                    o.Scope.Add("profile");
                    o.Scope.Add("email");
                });
            }
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
                    var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (string.IsNullOrEmpty(userId))
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
                        partitionKey: userId,
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

        #endregion

        #region Notification Services & App Services

        extension(IServiceCollection services)
        {
            /// <summary>
            /// Registers notification-related services (email queue, templates, dispatcher...).
            /// </summary>
            private void AddNotificationServices()
            {
                services.AddSingleton<IEmailBranding, DefaultBranding>();
                services.AddSingleton<IEmailQueue, EmailQueue>();
                services.AddSingleton<IEmailTransport, MailKitEmailTransport>();
                services.AddSingleton<IEmailTemplateRenderer, RazorTemplateRenderer>();
                services.AddScoped<IEmailService, EmailService>();
                services.AddHostedService<EmailDispatcher>();
            
                services.AddScoped<ISmsSender, HodhodSmsSender>();
                services.AddScoped<IEmailSender, EmailSenderViaEmailService>();
                services.AddHostedService<NotificationDispatcher>();
            }

            /// <summary>
            /// Registers general application services (storage clients, token services, etc).
            /// </summary>
            private void AddServices(IConfiguration configuration)
            {
                var connectionString = configuration["Storage:ConnectionString"] ?? string.Empty;
                var containerName = configuration["Storage:RootPath"] ?? string.Empty;

                services.AddSingleton(_ => new BlobServiceClient(connectionString));
                services.AddScoped(sp =>
                {
                    var serviceClient = sp.GetRequiredService<BlobServiceClient>();
                    var containerClient = serviceClient.GetBlobContainerClient(containerName);
                    return containerClient;
                });
                services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
                services.AddTransient<IExternalIdTokenValidator, AzureIdTokenValidator>();
                services.AddScoped<IFileStorageService, AzureBlobStorageService>();
                services.AddScoped<IProfileCompletenessService, ProfileCompletenessService>();
                services.AddScoped<IPasswordVerifier, PasswordVerifier>();

                services.AddScoped<ITokenService, TokenService>();
                services.AddScoped<IVerificationService, VerificationService>();
                services.AddScoped<ICurrentUserService, CurrentUserService>();

                services.AddScoped<IExternalTokenReader, CookieExternalTokenReader>();
                services.AddScoped<IMediaUrlResolver, MediaUrlResolver>();

                services.AddHttpClient<IRecaptchaService, RecaptchaService>();

                services.AddSingleton<IEnvironmentNameProvider, EnvironmentNameProvider>();
            
                services.AddSingleton<ILocalizationService, LocalizationService>();
                
                services.AddScoped<ISessionService, EfSessionService>();
            }
        }

        #endregion
    }
}
