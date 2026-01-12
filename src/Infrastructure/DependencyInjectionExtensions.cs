using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Threading.RateLimiting;
using Application.Operation.Common.Interfaces.Services.HttpClients;
using Application.Operation.Common.Repositories;
using Application.Operation.Common.Validations;
using Application.Operation.Templates.ChangeJobStatusNotification;
using Application.Operation.Templates.JobCandidateInvitationSent;
using Application.Operation.Templates.JobCreatedNotification;
using Application.Operation.Templates.JobDeletedNotification;
using Application.Operation.Templates.JobUpdatedNotification;
using Application.Recruitment.Common.Interfaces.Services.HttpClients;
using Application.Recruitment.Templates.ContactVerificationSent;
using Azure.Storage.Blobs;
using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Tawtheef.Application.Common.Interfaces.NotificationServices;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Common.Interfaces.Services.HttpClients;
using Tawtheef.Application.Common.Interfaces.Services.Notifications;
using Tawtheef.Application.Common.Interfaces.Services.Resources;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Application.Common.Security;
using Tawtheef.Application.Common.Validations;
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
using Tawtheef.Infrastructure.Utils;

namespace Tawtheef.Infrastructure
{
    /// <summary>
    /// Keys for rate limiting policies used across the application.
    /// </summary>
    public static class LimitsPolicyKeys
    {
        public const string VerificationRequestPolicy = "VerificationRequestPolicy";
        public const string VerificationConfirmationPolicy = "VerificationConfirmationPolicy";
        public const string MoiCheckProfilePolicy = "MoiCheckProfilePolicy";
    }

    /// <summary>
    /// Extension methods for registering infrastructure layer services.
    /// </summary>
    public static class DependencyInjectionExtensions
    {
        /// <summary>
        /// Registers all infrastructure services (DB, repos, auth, notifications, storage, etc).
        /// Split by App:Module (Recruitment / Operation / Both).
        /// </summary>
        public static void AddInfrastructureLayer(
            this IServiceCollection services,
            IConfiguration configuration,
            IHostEnvironment env)
        {
            // App mode
            var runtime = configuration.GetSection(AppRuntimeSettings.SectionName).Get<AppRuntimeSettings>()
                ?? throw new InvalidOperationException($"Configuration section '{AppRuntimeSettings.SectionName}' is missing.");

            // Always register runtime settings for DI usage
            services.AddOptions<AppRuntimeSettings>()
                .Bind(configuration.GetSection(AppRuntimeSettings.SectionName))
                .Validate(s => s.Module != 0, "App:Module is required. Allowed: Recruitment | Operation | Both.")
                .ValidateOnStart();

            // ===== Common (always) =====
            services.AddInfrastructureCommon(configuration, env);

            // ===== Recruitment-only =====
            if (runtime.Module is AppModule.Recruitment)
                services.AddInfrastructureRecruitment(configuration);

            // ===== Operation-only =====
            if (runtime.Module is AppModule.Operation)
                services.AddInfrastructureOperation(configuration);
        }

        #region Common

        extension(IServiceCollection services)
        {
            private void AddInfrastructureCommon(IConfiguration configuration,
                IHostEnvironment env)
            {
                // Feature flags (keep consistent with your json; using FeatureFlags here)
                services.AddFeatureManagement(configuration.GetSection("FeatureFlags"));

                // Options (Common)
                AddValidatedOptions<JwtSettings>(services, configuration, JwtSettings.SectionName);
                AddValidatedOptions<AppConfigSettings>(services, configuration, AppConfigSettings.SectionName);
                AddValidatedOptions<EmailSettings>(services, configuration, EmailSettings.SectionName);
                AddValidatedOptions<StorageSettings>(services, configuration, StorageSettings.SectionName);

                // Optional settings that you may want in both apps (no ValidateOnStart here)
                services.Configure<EmailDispatcherSettings>(configuration.GetSection(EmailDispatcherSettings.SectionName));

                // Data + Identity
                services.AddTawtheefDbContext(configuration, env);

                // Repositories (Common)
                services.AddCommonRepositories();

                // Authentication & Authorization (Common)
                services.AddAuthorizationAndAuthenticationCommon(configuration);

                // Storage (Common)
                services.AddStorageCommon(configuration);
                
                // HttpClients (Common)
                services.AddInfrastructureHttpClients(configuration);

                // Notification (Common)
                services.AddNotificationServicesCommon();

                // Validators
                services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

                services.AddScoped<IPasswordVerifier, PasswordVerifier>();
                services.AddScoped<ILoginAuditService, LoginAuditService>();
                services.AddScoped<ITokenService, TokenService>();
                services.AddScoped<ISessionService, EfSessionService>();

                services.AddScoped<ICurrentUserService, CurrentUserService>();
                services.AddScoped<IMediaUrlResolver, MediaUrlResolver>();
                services.AddSingleton<ILocalizationService, LocalizationService>();

                // Authorization handlers/policy provider (Common)
                services.AddSingleton<IAuthorizationHandler, ProfileCompletedHandler>();
                services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
                services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
                
                //TODO: should be operated in Recruitment module and Operation module
                services.AddScoped<IProfileCompletenessService, ProfileCompletenessService>();
                
                // Background (Common) - keep only what truly runs in both
                services.AddHostedService<EmailDispatcher>();
                services.AddHostedService<NotificationDispatcher>();
            }

            private void AddCommonRepositories()
            {
                services.AddTransient<IUnitOfWork, UnitOfWork>()
                    .AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>))
                    .AddTransient<IUserRepository, UserRepository>();
                // Recruitment-only repositories
                services.AddScoped<IUserProfileRepository, UserProfileRepository>();
                
    
                //TODO: should be operated in Recruitment module and Operation module
                services.AddScoped<IJobRepository, JobRepository>();
            }

            private void AddNotificationServicesCommon()
            {
                services.AddSingleton<IEmailBranding, DefaultBranding>();
                services.AddSingleton<IEmailQueue, EmailQueue>();
                services.AddSingleton<IEmailTransport, MailKitEmailTransport>();
                services.AddSingleton<IEmailTemplateRenderer, RazorTemplateRenderer>();

                services.AddScoped<ISmsSender, HodhodSmsSender>();
                services.AddScoped<IEmailSender, EmailSenderViaEmailService>();
                services.AddScoped<IPushSender, NullPushSender>();
                services.AddScoped<IEmailService, EmailService>();
            }

            private void AddStorageCommon(IConfiguration configuration)
            {
                var storageSettings = configuration.GetSection(StorageSettings.SectionName).Get<StorageSettings>()!;
                if (storageSettings.Provider == nameof(StorageProvider.AzureBlobStorage)) {
                    services.AddSingleton(_ => new BlobServiceClient(storageSettings.AzureConnectionString));
                    services.AddScoped(sp => {
                        var serviceClient = sp.GetRequiredService<BlobServiceClient>();
                        return serviceClient.GetBlobContainerClient(storageSettings.RootPath);
                    });

                    services.AddScoped<IFileStorageService, AzureBlobStorageService>();
                }
                else
                {
                    services.AddScoped<IFileStorageService, LocalStorageService>();
                }
            }
            
            private void AddInfrastructureHttpClients(IConfiguration configuration)
            {
                // ===== Hodhod SMS Gateway =====
                AddValidatedOptions<HodhodSmsSettings>(services, configuration, HodhodSmsSettings.SectionName);
                services.AddHttpClient<ISmsGatewayClient, HodhodSmsClient>((sp, client) =>
                {
                    var opt = sp.GetRequiredService<IOptions<HodhodSmsSettings>>().Value;
                    client.BaseAddress = new Uri(opt.BaseUrl);
                    client.Timeout = TimeSpan.FromSeconds(opt.TimeoutSeconds);
                });
            }
        }

        #endregion

        #region Recruitment

        extension(IServiceCollection services)
        {
            private void AddInfrastructureRecruitment(IConfiguration configuration)
            {
                // Recruitment-only options + http clients
                services.AddRecruitmentHttpClients(configuration);
                services.AddRecruitmentNotification();

                // Recruitment-only domain services
                services.AddScoped<IVerificationService, VerificationService>();
                services.AddScoped<IProfileStepValidationService, ProfileStepValidationService>();
                services.AddScoped<IProfileReviewService, ProfileReviewService>();

            }

            private void AddRecruitmentHttpClients(IConfiguration configuration)
            {
                // ===== Qatar Pass =====
                AddValidatedOptions<QatarPassAuthSettings>(services, configuration, QatarPassAuthSettings.SectionName);
                services.AddHttpClient<IQatarPassClient, QatarPassClient>((sp, client) =>
                {
                    var opt = sp.GetRequiredService<IOptions<QatarPassAuthSettings>>().Value;
                    client.BaseAddress = new Uri(opt.BaseUrl);
                    client.Timeout = TimeSpan.FromSeconds(opt.TimeoutSeconds);
                });

                // ===== Qatar Resident OTP Verification =====
                AddValidatedOptions<QatarResidentOtpSettings>(services, configuration, QatarResidentOtpSettings.SectionName);
                services.AddHttpClient<IQatarResidentVerificationClient, QatarResidentVerificationClient>((sp, client) =>
                {
                    var opt = sp.GetRequiredService<IOptions<QatarResidentOtpSettings>>().Value;
                    client.BaseAddress = new Uri(opt.BaseUrl);
                    client.Timeout = TimeSpan.FromSeconds(opt.TimeoutSeconds);
                });

                // ===== MOI Client =====
                AddValidatedOptions<MoiSettings>(services, configuration, MoiSettings.SectionName);
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
            
            private void AddRecruitmentNotification()
            {
                //TODO: registeration all template model here
                NotificationTemplateRegistry.Register<ContactVerificationSentModel>(nameof(ContactVerificationSent));
            }
        }

        #endregion

        #region Operation

        extension(IServiceCollection services)
        {
            private void AddInfrastructureOperation(IConfiguration configuration)
            {
                // Operation-only settings + http clients
                services.AddOperationHttpClients();
                services.AddOperationNotification();

                AddValidatedOptions<AzureAuthenticationSettings>(services, configuration, AzureAuthenticationSettings.SectionName);
                
                // Operation-only options (do not ValidateOnStart unless always present in Operation appsettings)
                services.Configure<HrServiceSettings>(configuration.GetSection(HrServiceSettings.SectionName));

                // Operation-only services
                services.AddScoped<IEmployeeProfileService, EmployeeProfileService>();
                
                services.AddScoped<IJobPointsRepository, JobPointsRepository>();
                services.AddScoped<IJobPointsConfigurationsRepository, JobPointsConfigurationsRepository>();
                services.AddScoped<IJobReviewAttachmentRepository, JobReviewAttachmentRepository>();
                services.AddScoped<IJobTabReviewNoteRepository, JobTabReviewNoteRepository>();
                services.AddScoped<IJobConditionRepository, JobConditionRepository>();
                services.AddScoped<IJobDegreeRepository, JobDegreeRepository>();
                services.AddScoped<IJobSkillRepository, JobSkillRepository>();
                services.AddScoped<IJobResponsibilityRepository, JobResponsibilityRepository>();
                services.AddScoped<IJobRequiredAttachmentRepository, JobRequiredAttachmentRepository>();
                
                services.AddScoped<IJobValidationService, JobValidationService>();
                
                // Recruitment-only background jobs
                services.AddHostedService<JobAutoClosureService>();
            }

            private void AddOperationHttpClients()
            {
                // ===== Employee Directory =====
                services.AddHttpClient<IEmployeeDirectoryClient, EmployeeDirectoryClient>((sp, client) =>
                {
                    var opt = sp.GetRequiredService<IOptions<HrServiceSettings>>().Value;
                    client.BaseAddress = new Uri(opt.BaseUrl);
                    client.Timeout = TimeSpan.FromSeconds(opt.TimeoutSeconds);
                });
            }

            private void AddOperationNotification()
            {
                NotificationTemplateRegistry.Register<JobCandidateInvitationSentModel>(nameof(JobCandidateInvitationSent));
                NotificationTemplateRegistry.Register<JobCreatedNotificationModel>(nameof(JobCreatedNotification));
                NotificationTemplateRegistry.Register<JobUpdatedNotificationModel>(nameof(JobUpdatedNotification));
                NotificationTemplateRegistry.Register<JobDeletedNotificationModel>(nameof(JobDeletedNotification));
                NotificationTemplateRegistry.Register<ChangeJobStatusNotificationModel>(nameof(ChangeJobStatusNotification));
            }
        }

        #endregion

        #region DbContext & Identity

        private static void AddTawtheefDbContext(
            this IServiceCollection services,
            IConfiguration configuration,
            IHostEnvironment env)
        {
            services.AddScoped<AuditableEntityInterceptor>();
            services.AddScoped<DispatchDomainEventsInterceptor>();

            services.AddDbContext<TawtheefDbContext>((sp, options) =>
            {
                var cs = configuration.GetConnectionString(ConnectionStringSettings.SectionName);

                options.UseSqlServer(cs, sql =>
                    {
                        sql.MigrationsAssembly(typeof(TawtheefDbContext).Assembly.FullName);
                        sql.EnableRetryOnFailure(5);
                    })
                    .AddInterceptors(
                        sp.GetRequiredService<AuditableEntityInterceptor>(),
                        sp.GetRequiredService<DispatchDomainEventsInterceptor>());

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

        #region Authentication / Authorization / RateLimit

        extension(IServiceCollection services)
        {
            
            //TODO: should be operated in Recruitment module and Operation module
            private void AddAuthorizationAndAuthenticationCommon(IConfiguration configuration)
            {
                services.Configure<CookiePolicyOptions>(options =>
                {
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

                var jwtSettings = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()!;
                services.AddAuthentication(options =>
                    {
                        options.DefaultScheme = AuthSchemes.Smart;
                        options.DefaultAuthenticateScheme = AuthSchemes.Smart;
                        options.DefaultChallengeScheme = AuthSchemes.Smart;
                    })
                    .AddPolicyScheme(AuthSchemes.Smart, "Cookie or Bearer", o =>
                    {
                        o.ForwardDefaultSelector = ctx =>
                            ctx.Request.Headers.TryGetValue("Authorization", out var h) &&
                            h.ToString().StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                                ? JwtBearerDefaults.AuthenticationScheme
                                : AuthSchemes.AppCookieScheme;
                    })
                    .AddCookie(AuthSchemes.AppCookieScheme, o =>
                    {
                        o.Cookie.Name = AuthSchemes.AppCookieName;
                        o.Cookie.SameSite = SameSiteMode.None;
                        o.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                        o.SlidingExpiration = true;
                    })
                    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                    {
                        options.RequireHttpsMetadata = true;
                        options.SaveToken = true;
                        options.TokenValidationParameters = jwtSettings.ToTokenValidationParameters();

                        options.Events = new JwtBearerEvents
                        {
                            OnTokenValidated = async ctx =>
                            {
                                var sidFromToken =
                                    ctx.Principal?.FindFirst("sid")?.Value ??
                                    ctx.Principal?.FindFirst("http://schemas.microsoft.com/ws/2008/06/identity/claims/sid")?.Value;

                                var userId = ctx.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                                if (string.IsNullOrWhiteSpace(sidFromToken) || string.IsNullOrWhiteSpace(userId))
                                {
                                    ctx.Fail("Missing sid or user id.");
                                    return;
                                }

                                var sessionService = ctx.HttpContext.RequestServices.GetRequiredService<ISessionService>();
                                var currentSid = await sessionService.GetCurrentAsync(
                                    Guid.Parse(userId),
                                    ctx.HttpContext.RequestAborted);

                                if (string.IsNullOrWhiteSpace(currentSid) ||
                                    !string.Equals(currentSid, sidFromToken, StringComparison.Ordinal))
                                {
                                    ctx.Fail("Session changed. Please sign in again.");
                                }
                            }
                        };
                    });

                // ===== Google external login (optional) =====
                var googleSettings = configuration.GetSection(GoogleAuthenticationSettings.SectionName).Get<GoogleAuthenticationSettings>();
                if (googleSettings is not null)
                {
                    services.AddAuthentication().AddGoogle(options =>
                    {
                        options.ClientId = googleSettings.ClientId;
                        options.ClientSecret = googleSettings.ClientSecret;
                        options.SignInScheme = IdentityConstants.ExternalScheme;
                        options.SaveTokens = true;
                        options.Scope.Add("email");
                        options.Scope.Add("profile");
                        options.ClaimActions.MapJsonKey("picture", "picture");
                    });
                }

                // ===== Azure OIDC (optional) =====
                var azureSettings = configuration.GetSection(AzureAuthenticationSettings.SectionName).Get<AzureAuthenticationSettings>();
                if (azureSettings is not null)
                {
                    services.AddAuthentication()
                        .AddMicrosoftIdentityWebApp(configuration, AzureAuthenticationSettings.SectionName,
                            openIdConnectScheme: AuthSchemes.AzureOidc);

                    services.PostConfigure<OpenIdConnectOptions>(AuthSchemes.AzureOidc, o =>
                    {
                        o.SignInScheme = IdentityConstants.ExternalScheme;
                        o.ResponseType = OpenIdConnectResponseType.Code;
                        o.SaveTokens = true;
                        o.Scope.Add("openid");
                        o.Scope.Add("profile");
                        o.Scope.Add("email");
                    });
                    
                    services.AddSingleton<IConfigurationManager<OpenIdConnectConfiguration>>(sp => {
                        var tenant = string.IsNullOrWhiteSpace(azureSettings.TenantId) ? "common" : azureSettings.TenantId;
                        var authority = $"{azureSettings.Instance}{tenant}/v2.0";
                        var metadataAddress = $"{authority}/.well-known/openid-configuration";

                        var retriever = new HttpDocumentRetriever { RequireHttps = true };

                        return new ConfigurationManager<OpenIdConnectConfiguration>(
                            metadataAddress,
                            new OpenIdConnectConfigurationRetriever(),
                            retriever);
                    });
                    // Common services
                    services.AddTransient<IExternalIdTokenValidator, AzureIdTokenValidator>();
                }

                // Rate limiting + authorization policies
                services.AddRateLimitingPoliciesCommon();
                services.AddAuthorizationPoliciesCommon();
            }

            private void AddRateLimitingPoliciesCommon()
            {
                services.AddRateLimiter(options =>
                {
                    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                    options.AddPolicy(LimitsPolicyKeys.VerificationRequestPolicy, context =>
                    {
                        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                        var key = !string.IsNullOrWhiteSpace(userId)
                            ? $"user:{userId}"
                            : $"ip:{context.Connection.RemoteIpAddress}";

                        return RateLimitPartition.GetFixedWindowLimiter(
                            partitionKey: key,
                            factory: _ => new FixedWindowRateLimiterOptions
                            {
                                PermitLimit = 5,
                                Window = TimeSpan.FromMinutes(10),
                                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                                QueueLimit = 0
                            });
                    });

                    options.AddPolicy(LimitsPolicyKeys.VerificationConfirmationPolicy, context =>
                    {
                        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                        var key = !string.IsNullOrWhiteSpace(userId)
                            ? $"user:{userId}"
                            : $"ip:{context.Connection.RemoteIpAddress}";

                        return RateLimitPartition.GetFixedWindowLimiter(
                            partitionKey: key,
                            factory: _ => new FixedWindowRateLimiterOptions
                            {
                                PermitLimit = 10,
                                Window = TimeSpan.FromMinutes(10),
                                QueueLimit = 0
                            });
                    });

                    options.AddPolicy(LimitsPolicyKeys.MoiCheckProfilePolicy, context =>
                    {
                        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                        var key = !string.IsNullOrWhiteSpace(userId)
                            ? $"user:{userId}"
                            : $"ip:{context.Connection.RemoteIpAddress}";

                        return RateLimitPartition.GetFixedWindowLimiter(
                            partitionKey: key,
                            factory: _ => new FixedWindowRateLimiterOptions
                            {
                                PermitLimit = 6,
                                Window = TimeSpan.FromMinutes(5),
                                QueueLimit = 0,
                                QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                            });
                    });
                });
            }

            private void AddAuthorizationPoliciesCommon()
            {
                services.AddAuthorization();
            }
        }

        #endregion

        #region Options Helper

        private static void AddValidatedOptions<T>(
            IServiceCollection services,
            IConfiguration configuration,
            string sectionName) where T : class
        {
            services.AddOptions<T>()
                .Bind(configuration.GetSection(sectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();
        }

        #endregion
    }
}
