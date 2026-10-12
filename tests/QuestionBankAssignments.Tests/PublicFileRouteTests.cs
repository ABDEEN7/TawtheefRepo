using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using Application.Operation.Features.Employee.QuestionBankAssignments.Queries;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Tawtheef.Application.Common.Interfaces.Services.Resources;
using Tawtheef.Application.Common.Security;
using Tawtheef.Application.Features.Resources.Handlers.Queries;
using Tawtheef.Application.Features.Resources.Queries;
using Tawtheef.Domain.Configurations.Settings;

namespace QuestionBankAssignments.Tests;

// Real HTTP routing, authorization middleware, production controllers and local
// storage. MediatR dispatch is wired to the production file handlers; database
// authorization decisions are covered separately by QuestionImageAccessTests.
[TestFixture("Operations")]
[TestFixture("Recruitment")]
public class PublicFileRouteTests(string api)
{
    private const string SigningKey = "local-regression-test-signing-key";
    private PublicStorageFixture _files = null!;
    private WebApplication _app = null!;
    private HttpClient _client = null!;
    private Mock<IMediator> _mediator = null!;

    [OneTimeSetUp]
    public async Task StartApi()
    {
        _files = new PublicStorageFixture();
        var controller = api == "Operations"
            ? typeof(Operations.API.Controllers.ResourcesController)
            : typeof(Recruitment.API.Controllers.ResourcesController);
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            EnvironmentName = "Testing", ContentRootPath = _files.DirectoryPath,
            ApplicationName = typeof(PublicFileRouteTests).Assembly.FullName
        });
        builder.Logging.ClearProviders();
        builder.WebHost.UseUrls("http://127.0.0.1:0");
        builder.Services.AddControllers().ConfigureApplicationPartManager(parts =>
        {
            parts.ApplicationParts.Clear();
            parts.ApplicationParts.Add(new AssemblyPart(controller.Assembly));
            parts.FeatureProviders.Add(new OnlyResourcesController(controller));
        });
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddScheme<AuthenticationSchemeOptions, ResourceTestAuthenticationHandler>(JwtBearerDefaults.AuthenticationScheme, _ => { });
        builder.Services.AddAuthorization();
        var context = new HttpContextAccessor();
        builder.Services.AddSingleton<IHttpContextAccessor>(context);
        builder.Services.AddSingleton<IFileStorageService>(_files.Storage);
        _mediator = new Mock<IMediator>();
        var publicHandler = new GetPublicFileHandler(_files.Storage);
        _mediator.Setup(x => x.Send(It.IsAny<GetPublicFileQuery>(), It.IsAny<CancellationToken>()))
            .Returns((GetPublicFileQuery query, CancellationToken ct) => publicHandler.Handle(query, ct));
        var signedHandler = new GetSignedBlobHandler(_files.Storage, _files.Logger.Object, Options.Create(_files.Settings),
            Options.Create(new AppConfigSettings
            {
                FrontendUrl = "http://localhost", BackendUrl = "http://localhost", BlobSignKey = SigningKey,
                AdminEmail = "reviewer@example.test", AdminEmails = ["reviewer@example.test"], DefaultSignedUrlMinutes = 3
            }));
        _mediator.Setup(x => x.Send(It.IsAny<GetSignedBlobQuery>(), It.IsAny<CancellationToken>()))
            .Returns((GetSignedBlobQuery query, CancellationToken ct) => signedHandler.Handle(query, ct));
        _mediator.Setup(x => x.Send(It.IsAny<CheckQuestionBankImageAccessQuery>(), It.IsAny<CancellationToken>()))
            .Returns((CheckQuestionBankImageAccessQuery _, CancellationToken _) =>
                Task.FromResult<IResult<Unit>>(context.HttpContext!.User.HasClaim("permission", PermissionKeys.QuestionBankRequests.Review)
                    ? Result.Ok(Unit.Value)
                    : Result.Fail<Unit>(new Error("Forbidden").WithMetadata("StatusCode", 403))));
        builder.Services.AddSingleton(_mediator.Object);
        _app = builder.Build();
        _app.UseRouting();
        _app.UseAuthentication();
        _app.UseAuthorization();
        _app.MapControllers();
        await _app.StartAsync();
        var address = _app.Services.GetRequiredService<IServer>().Features.Get<IServerAddressesFeature>()!.Addresses.Single();
        _client = new HttpClient(new HttpClientHandler { UseProxy = false }) { BaseAddress = new Uri(address) };
    }

    [OneTimeTearDown]
    public async Task StopApi()
    {
        _client?.Dispose();
        if (_app != null)
        {
            await _app.StopAsync();
            await _app.DisposeAsync();
        }
        _files?.Dispose();
    }

    [Test]
    public async Task AnonymousPublicImagesStillSupportEncodedPathsCachingAndRangeRequests()
    {
        var path = "/files/" + Uri.EscapeDataString(PublicStorageFixture.PublicKey["public/".Length..]);
        using var response = await _client.GetAsync(path);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(await response.Content.ReadAsStringAsync(), Is.EqualTo(PublicStorageFixture.PublicBody));
        Assert.That(response.Content.Headers.ContentType!.MediaType, Is.EqualTo("image/png"));
        Assert.That(response.Headers.ETag, Is.Not.Null);
        Assert.That(response.Content.Headers.LastModified, Is.Not.Null);

        using var rangeRequest = new HttpRequestMessage(HttpMethod.Get, path);
        rangeRequest.Headers.Range = new RangeHeaderValue(0, 4);
        using var range = await _client.SendAsync(rangeRequest);
        Assert.That(range.StatusCode, Is.EqualTo(HttpStatusCode.PartialContent));
        Assert.That(await range.Content.ReadAsStringAsync(), Is.EqualTo(PublicStorageFixture.PublicBody[..5]));

        using var cachedRequest = new HttpRequestMessage(HttpMethod.Get, path);
        cachedRequest.Headers.IfNoneMatch.Add(response.Headers.ETag!);
        using var cached = await _client.SendAsync(cachedRequest);
        Assert.That(cached.StatusCode, Is.EqualTo(HttpStatusCode.NotModified));
    }

    [Test]
    public async Task PublicRequestsCannotReachKnownPrivateImagesOrOutsideStorage()
    {
        var privatePath = _files.PrivateKey;
        var privateBackslashes = privatePath.Replace('/', '\\');
        var resourceId = Path.GetFileName(privatePath).Split('_')[0];
        string[] paths =
        [
            "../" + privatePath,
            "..\\" + privateBackslashes,
            "folder/..\\../" + privatePath,
            "%2e%2e%2f" + privatePath,
            "%2E%2E%5C" + Uri.EscapeDataString(privateBackslashes),
            "%252e%252e%252f" + privatePath,
            "%25252e%25252e%25252f" + privatePath,
            "..%20/" + privatePath,
            "../..//storage-other/outside.png",
            "..%5c..%2fstorage-other/outside.png",
            Uri.EscapeDataString(Path.Combine(_files.Root, privatePath)),
            Uri.EscapeDataString("C:\\storage\\" + privateBackslashes),
            "%2fetc/passwd", "%5c%5cserver/share/image.png", "%00image.png",
            privatePath, Uri.EscapeDataString(privatePath), resourceId,
            Guid.Parse(resourceId).ToString(), WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(privatePath)),
            Uri.EscapeDataString("http://localhost/api/resources/" + privatePath)
        ];
        foreach (var path in paths)
        {
            using var response = await _client.GetAsync("/files/" + path);
            Assert.That(response.StatusCode, Is.AnyOf(HttpStatusCode.NotFound, HttpStatusCode.BadRequest), path);
            Assert.That(await response.Content.ReadAsStringAsync(), Does.Not.Contain(PublicStorageFixture.PrivateBody), path);
        }

        // An authenticated reviewer still cannot use the public route as a
        // substitute for the secured resource endpoint.
        using var authenticatedRequest = new HttpRequestMessage(HttpMethod.Get, "/files/%2e%2e%2f" + privatePath);
        authenticatedRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "reviewer-test");
        using var authenticated = await _client.SendAsync(authenticatedRequest);
        Assert.That(authenticated.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task PublicSymlinkCannotServePrivateImage()
    {
        var alias = "http-alias-" + Guid.NewGuid().ToString("N") + ".png";
        File.CreateSymbolicLink(Path.Combine(_files.Root, "public", alias), Path.Combine(_files.Root, _files.PrivateKey));
        using var response = await _client.GetAsync("/files/" + alias);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task PrivateImagesRequireAuthenticationAndRemainAccessibleThroughSecuredEndpoints()
    {
        var encoded = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(_files.PrivateKey));
        var expiration = DateTimeOffset.UtcNow.AddMinutes(3).ToUnixTimeSeconds();
        var signature = WebEncoders.Base64UrlEncode(HMACSHA256.HashData(Encoding.UTF8.GetBytes(SigningKey), Encoding.UTF8.GetBytes($"{encoded}.{expiration}")));
        foreach (var path in new[] { "/api/resources/" + encoded, $"/api/resources/dl?b={encoded}&exp={expiration}&sig={signature}" })
        {
            using var anonymous = await _client.GetAsync(path);
            Assert.That(anonymous.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
            using var request = new HttpRequestMessage(HttpMethod.Get, path);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "reviewer-test");
            using var authorized = await _client.SendAsync(request);
            Assert.That(authorized.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(await authorized.Content.ReadAsStringAsync(), Is.EqualTo(PublicStorageFixture.PrivateBody));
            if (api == "Operations") Assert.That(authorized.Headers.CacheControl!.NoStore, Is.True);
        }
        if (api == "Operations")
            _mediator.Verify(x => x.Send(It.Is<CheckQuestionBankImageAccessQuery>(q => q.BlobKey == _files.PrivateKey), It.IsAny<CancellationToken>()), Times.AtLeast(2));

        using var tamperedRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/resources/dl?b={encoded}&exp={expiration}&sig=invalid");
        tamperedRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "reviewer-test");
        using var tampered = await _client.SendAsync(tamperedRequest);
        Assert.That((int)tampered.StatusCode, Is.GreaterThanOrEqualTo(400));
        Assert.That(await tampered.Content.ReadAsStringAsync(), Does.Not.Contain(PublicStorageFixture.PrivateBody));

        if (api == "Operations")
        {
            foreach (var path in new[] { "/api/resources/" + encoded, $"/api/resources/dl?b={encoded}&exp={expiration}&sig={signature}" })
            {
                using var deniedRequest = new HttpRequestMessage(HttpMethod.Get, path);
                deniedRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "employee-denied-test");
                using var denied = await _client.SendAsync(deniedRequest);
                Assert.That(denied.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
                Assert.That(await denied.Content.ReadAsStringAsync(), Does.Not.Contain(PublicStorageFixture.PrivateBody));
            }
        }
    }

    private sealed class OnlyResourcesController(Type controller) : IApplicationFeatureProvider<ControllerFeature>
    {
        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            foreach (var candidate in feature.Controllers.Where(c => c.AsType() != controller).ToArray())
                feature.Controllers.Remove(candidate);
        }
    }
}

public sealed class ResourceTestAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var reviewer = Request.Headers.Authorization == "Bearer reviewer-test";
        if (!reviewer && Request.Headers.Authorization != "Bearer employee-denied-test")
            return Task.FromResult(AuthenticateResult.NoResult());
        var identity = new ClaimsIdentity(reviewer ? [new Claim("permission", PermissionKeys.QuestionBankRequests.Review)] : [], Scheme.Name);
        return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(identity), Scheme.Name)));
    }
}
