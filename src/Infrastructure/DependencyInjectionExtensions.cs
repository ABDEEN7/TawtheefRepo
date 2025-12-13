using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.RateLimiting;
using Azure.Storage.Blobs;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Tawtheef.Application.Common.Constants;
using Tawtheef.Application.Common.Interfaces.NotificationServices;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Common.Interfaces.Services.HttpClients;
using Tawtheef.Application.Common.Interfaces.Validations;
using Tawtheef.Domain.Configurations.Settings;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Infrastructure.Data;
using Tawtheef.Infrastructure.Data.Interceptors;
using Tawtheef.Infrastructure.Repositories;
using Tawtheef.Infrastructure.Repositories.Base;
using Tawtheef.Infrastructure.Services.Authorization;
using Tawtheef.Infrastructure.Services.BackgroundJobs;
using Tawtheef.Infrastructure.Services.HttpClients;
using Tawtheef.Infrastructure.Services.Identity;
using Tawtheef.Infrastructure.Services.Localization;
using Tawtheef.Infrastructure.Services.NotificationServices;
using Tawtheef.Infrastructure.Services.StorageServices;
using Tawtheef.Infrastructure.Services.Validations;

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
        public static void AddInfrastructureLayer(this IServiceCollection services, IConfiguration configuration, IHostEnvironment env)
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
            services.AddBackgroundServices(configuration);

            // Data & Repositories
            services.AddDbContext(configuration, env);
            services.AddRepositories();

            // Authentication & Authorization
            services.AddAuthorizationAndAuthentication(configuration);
            
            RegisterHttpClients(services, configuration);
        }

        #region Configuration Helpers

        private static void ConfigureOptions(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
            services.Configure<AppConfigSettings>(configuration.GetSection(AppConfigSettings.SectionName));
            services.Configure<EmailSettings>(configuration.GetSection(EmailSettings.SectionName));
            services.Configure<StorageSettings>(configuration.GetSection(StorageSettings.SectionName));
            services.Configure<EmailDispatcherSettings>(configuration.GetSection(EmailDispatcherSettings.SectionName));
        }

        #endregion

        #region DbContext & Identity

        /// <summary>
        /// Adds the TawtheefDbContext and Identity configuration.
        /// </summary>
        private static void AddDbContext(this IServiceCollection services, IConfiguration configuration,
            IHostEnvironment env)
        {
            // Connection string kept for potential conditional logic later
            services.AddScoped<AuditableEntityInterceptor>();

            services.AddDbContext<TawtheefDbContext>((sp, options) =>
            {
                var cs = configuration.GetConnectionString(ConnectionStringSettings.SectionName);

                // register interceptors or other options as needed
                options.UseSqlServer(cs, sql => {
                        sql.MigrationsAssembly(typeof(TawtheefDbContext).Assembly.FullName);
                        sql.EnableRetryOnFailure(5);
                    })
                    .AddInterceptors(sp.GetRequiredService<AuditableEntityInterceptor>());
                
                if (env.IsDevelopment())
                {
                    options
                        .EnableSensitiveDataLogging()
                        .EnableDetailedErrors()
                        .LogTo(Console.WriteLine, LogLevel.Information);
                }
            });

            services.AddIdentity<User, ApplicationRole>()
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
                .AddTransient<IUserRepository, UserRepository>()
                .AddScoped<IJobRepository, JobRepository>()
                .AddScoped<IJobConditionRepository, JobConditionRepository>()
                .AddScoped<IJobDegreeRepository, JobDegreeRepository>()
                .AddScoped<IJobQuotaRepository, JobQuotaRepository>()
                .AddScoped<IJobSkillRepository, JobSkillRepository>()
                .AddScoped<IJobResponsibilityRepository, JobResponsibilityRepository>()
                .AddScoped<IJobRequiredAttachmentRepository, JobRequiredAttachmentRepository>()
                .AddScoped<IResidentsBreakdownRepository, ResidentsBreakdownRepository>();
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
            // ===== reCAPTCHA =====
            services.Configure<RecaptchaSettings>(configuration.GetSection(RecaptchaSettings.SectionName));
            services.AddHttpClient<IRecaptchaService, RecaptchaService>((sp, client) =>
            {
                var opt = sp.GetRequiredService<IOptions<RecaptchaSettings>>().Value;
                client.BaseAddress = new Uri(opt.BaseUrl);
                client.Timeout = TimeSpan.FromSeconds(opt.TimeoutSeconds);
            });

            // ===== Qatar Pass =====
            services.Configure<QatarPassAuthSettings>(configuration.GetSection(QatarPassAuthSettings.SectionName));
            services.AddHttpClient<IQatarPassClient, QatarPassClient>((sp, client) =>
            {
                var opt = sp.GetRequiredService<IOptions<QatarPassAuthSettings>>().Value;
                client.BaseAddress = new Uri(opt.BaseUrl);
                client.Timeout = TimeSpan.FromSeconds(opt.TimeoutSeconds);
            });

            // ===== Hodhod SMS =====
            services.Configure<HodhodSmsSettings>(configuration.GetSection(HodhodSmsSettings.SectionName));
            services.AddHttpClient<ISmsGatewayClient, HodhodSmsClient>((sp, client) =>
            {
                var opt = sp.GetRequiredService<IOptions<HodhodSmsSettings>>().Value;
                client.BaseAddress = new Uri(opt.BaseUrl);
                client.Timeout = TimeSpan.FromSeconds(opt.TimeoutSeconds);
            });

            // ===== MOI Client =====
            services.Configure<MoiSettings>(configuration.GetSection(MoiSettings.SectionName));
            services.AddHttpClient<IMoiClient, MoiClient>((sp, client) =>
                {
                    var opt = sp.GetRequiredService<IOptions<MoiSettings>>().Value;
                    client.BaseAddress = new Uri(opt.BaseUrl);
                    client.Timeout = TimeSpan.FromSeconds(opt.TimeoutSeconds);
                })
                .ConfigurePrimaryHttpMessageHandler(sp =>
                {
                    var opt = sp.GetRequiredService<IOptions<MoiSettings>>().Value;

                    return new HttpClientHandler
                    {
                        Credentials = new NetworkCredential(opt.Username, opt.Password),
                        UseCookies = true,
                        CookieContainer = new CookieContainer(),
                        PreAuthenticate = false,
                        UseDefaultCredentials = false
                    };
                });
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

            var jwtSection = configuration.GetSection(JwtSettings.SectionName);
            var issuer = jwtSection[nameof(JwtSettings.Issuer)]!;
            var audience = jwtSection[nameof(JwtSettings.Audience)]!;
            var signingKeyRaw = jwtSection[nameof(JwtSettings.SigningKey)]!;
            
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKeyRaw));

            services.AddAuthentication(options => {
                    options.DefaultScheme = AuthSchemes.Smart;
                    options.DefaultAuthenticateScheme = AuthSchemes.Smart;
                    options.DefaultChallengeScheme = AuthSchemes.Smart;
                })
                .AddPolicyScheme(AuthSchemes.Smart, "Cookie or Bearer", o => {
                    o.ForwardDefaultSelector = ctx =>
                        ctx.Request.Headers.TryGetValue("Authorization", out var h) &&
                        h.ToString().StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                            ? JwtBearerDefaults.AuthenticationScheme : AuthSchemes.AppCookieScheme;
                })
                .AddCookie(AuthSchemes.AppCookieScheme,o => {
                    o.Cookie.Name = AuthSchemes.AppCookieName;
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

            var google = configuration.GetSection(GoogleAuthenticationSettings.SectionName);
            var googleClientId = google[nameof(GoogleAuthenticationSettings.ClientId)];
            var googleClientSecret = google[nameof(GoogleAuthenticationSettings.ClientSecret)];
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
                    options.ClaimActions.MapJsonKey("picture", "picture"); 
                });
            }

            var azure = configuration.GetSection(AzureAuthenticationSettings.SectionName);
            var azureClientId = azure[nameof(AzureAuthenticationSettings.ClientId)];
            var azureTenantId = azure[nameof(AzureAuthenticationSettings.TenantId)];
            var azureInstance = azure[nameof(AzureAuthenticationSettings.Instance)];

            var azureEnabled =
                !string.IsNullOrWhiteSpace(azureClientId) &&
                !string.IsNullOrWhiteSpace(azureTenantId) &&
                !string.IsNullOrWhiteSpace(azureInstance);

            if (azureEnabled)
            {
                services.AddAuthentication()
                    .AddMicrosoftIdentityWebApp(configuration, AzureAuthenticationSettings.SectionName,
                        openIdConnectScheme: AuthSchemes.AzureOidc);

                services.PostConfigure<OpenIdConnectOptions>(AuthSchemes.AzureOidc, o => {
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
            
                services.AddScoped<ISmsSender, HodhodSmsSender>();
                services.AddScoped<IEmailSender, EmailSenderViaEmailService>();
            }

            /// <summary>
            /// Registers general application services (storage clients, token services, etc).
            /// </summary>
            private void AddServices(IConfiguration configuration)
            {
                var connectionString = configuration[$"{StorageSettings.SectionName}:{nameof(StorageSettings.AzureConnectionString)}"] ?? string.Empty;
                var containerName = configuration[$"{StorageSettings.SectionName}:{nameof(StorageSettings.RootPath)}"] ?? string.Empty;
                if (!string.IsNullOrEmpty(connectionString))
                {
                    services.AddSingleton(_ => new BlobServiceClient(connectionString));
                    services.AddScoped(sp =>
                    {
                        var serviceClient = sp.GetRequiredService<BlobServiceClient>();
                        var containerClient = serviceClient.GetBlobContainerClient(containerName);
                        return containerClient;
                    });
                    services.AddScoped<IFileStorageService, AzureBlobStorageService>();
                }
                else
                {
                    services.AddScoped<IFileStorageService, LocalStorageService>();
                }

                services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
                
                services.AddTransient<IExternalIdTokenValidator, AzureIdTokenValidator>();
                services.AddScoped<IPasswordVerifier, PasswordVerifier>();
                services.AddScoped<ILoginAuditService, LoginAuditService>();
                services.AddScoped<ITokenService, TokenService>();
                services.AddScoped<ISessionService, EfSessionService>();
                
                services.AddScoped<IVerificationService, VerificationService>();
                services.AddScoped<ICurrentUserService, CurrentUserService>();
                
                services.AddScoped<IMediaUrlResolver, MediaUrlResolver>();
                services.AddSingleton<ILocalizationService, LocalizationService>();

                services.AddScoped<IProfileStepValidationService, ProfileStepValidationService>();
                services.AddScoped<IJobValidationService, JobValidationService>();
                services.AddScoped<IProfileReviewService, ProfileReviewService>();
                services.AddScoped<IProfileCompletenessService, ProfileCompletenessService>();
            }
        }
        
        public static void AddBackgroundServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHostedService<EmailDispatcher>();
            services.AddHostedService<NotificationDispatcher>();
            services.AddHostedService<JobAutoClosureService>();
        }
        
        #endregion
        
        private static void ConfigureAuthorizationPolicies(IServiceCollection services)
        {
            services.AddSingleton<IAuthorizationHandler, ProfileCompletedHandler>();
            services.AddSingleton<IAuthorizationHandler, PermissionHandler>();
            services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
        }
    }
}
