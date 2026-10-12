using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Tawtheef.Application.Common.Interfaces.Logging;
using Tawtheef.Application.Common.Interfaces.Services.Resources;
using Tawtheef.Application.Common.Services;
using Tawtheef.Application.Features.Resources.Handlers.Queries;
using Tawtheef.Application.Features.Resources.Queries;
using Tawtheef.Domain.Configurations.Settings;
using Tawtheef.Infrastructure.Services.StorageServices;

namespace QuestionBankAssignments.Tests;

internal sealed class PublicStorageFixture : IDisposable
{
    public string DirectoryPath { get; } = Path.Combine(Path.GetTempPath(), "tawtheef-public-files-" + Guid.NewGuid());
    public string Root => Path.Combine(DirectoryPath, "storage");
    public const string PublicKey = "public/logos/public image.png";
    public string PrivateKey { get; } = LocalPathBuilder.QuestionImage(Guid.NewGuid(), Guid.NewGuid(), "png", "testhash");
    public const string PublicBody = "legitimate public image";
    public const string PrivateBody = "PRIVATE QUESTION BANK IMAGE";
    public Mock<IAppLogger> Logger { get; } = new();
    public StorageSettings Settings { get; }
    public LocalStorageService Storage { get; }

    public PublicStorageFixture()
    {
        Settings = new StorageSettings
        {
            Provider = nameof(StorageProvider.Local), RootPath = Root, PublicBaseUrl = "http://localhost/files"
        };
        Logger.Setup(x => x.ForContext(It.IsAny<Type>())).Returns(Logger.Object);
        Storage = new LocalStorageService(Options.Create(Settings), Logger.Object);
        Write(PublicKey, PublicBody);
        Write(PrivateKey, PrivateBody);
        var sibling = Path.Combine(DirectoryPath, "storage-other");
        Directory.CreateDirectory(sibling);
        File.WriteAllText(Path.Combine(sibling, "outside.png"), PrivateBody);
    }

    public void Write(string key, string body)
    {
        var path = Path.Combine(Root, key.Replace('/', Path.DirectorySeparatorChar));
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, body);
    }

    public void Dispose() => Directory.Delete(DirectoryPath, recursive: true);
}

public class PublicFileStorageTests
{
    public static readonly string[] UnsafeKeys =
    [
        "public/../private/image.png", "public\\..\\private\\image.png", "public/folder/..\\../private/image.png",
        "public/../../storage-other/outside.png",
        "public/./image.png", "public/.. /private/image.png", "public/.../private/image.png",
        "public/%2e%2e/private/image.png", "public/%252e%252e/private/image.png",
        "public/%2Fprivate/image.png", "/public/image.png", "\\public\\image.png",
        "C:/public/image.png", "public/C:/image.png", "//server/share/image.png",
        "public/\\?\\C:\\image.png", "public/image.png:secret", "public/image.png.", "public/image.png ",
        "public/image\0.png", "public/image\r\n.png", "public/CON.png", "public/CON .png",
        "public/NUL", "public/LPT1.png", "public/COM¹.png", "public/image.png/", "other/image.png"
    ];

    [TestCaseSource(nameof(UnsafeKeys))]
    public async Task UnsafeKeysCannotMapReadWriteDeleteOrPublishFiles(string key)
    {
        using var fixture = new PublicStorageFixture();
        Assert.That(fixture.Storage.MapPath(key).IsFailed, Is.True);
        Assert.That(await fixture.Storage.OpenReadAsync(key, default), Is.Null);
        Assert.That(await fixture.Storage.ExistsAsync(key, default), Is.False);
        Assert.That(fixture.Storage.ToPublicUrl(key).IsFailed, Is.True);
        using var body = new MemoryStream(Encoding.UTF8.GetBytes("overwrite attempt"));
        Assert.That((await fixture.Storage.SaveAsync(body, key)).IsFailed, Is.True);
        Assert.That((await fixture.Storage.DeleteAsync(key)).IsFailed, Is.True);
        Assert.That(File.ReadAllText(Path.Combine(fixture.Root, fixture.PrivateKey)), Is.EqualTo(PublicStorageFixture.PrivateBody));
    }

    [TestCase("../private/image.png")]
    [TestCase("..\\private\\image.png")]
    [TestCase("folder/..\\../private/image.png")]
    [TestCase("%2e%2e%2fprivate%2fimage.png")]
    [TestCase("%2E%2E%5Cprivate%5Cimage.png")]
    [TestCase("%252e%252e%252fprivate/image.png")]
    [TestCase("%25252e%25252e%25252fprivate/image.png")]
    [TestCase("%c0%ae%c0%ae%2fprivate/image.png")]
    [TestCase("%2fetc/passwd")]
    [TestCase("%5c%5cserver/share/image.png")]
    [TestCase("C%3a%5cprivate/image.png")]
    [TestCase("%00image.png")]
    [TestCase("..%20/private/image.png")]
    public void PublicRoutesRejectDecodedAndMultiplyEncodedTraversal(string path)
        => Assert.That(StoragePathRules.TryPublicKey(path, out _), Is.False);

    [Test]
    public async Task PublicUrlsRoundTripExistingGeneratedKeysAndKeepFileMetadata()
    {
        using var fixture = new PublicStorageFixture();
        string[] keys =
        [
            PublicStorageFixture.PublicKey,
            LocalPathBuilder.UniversityLogo(Guid.NewGuid(), "main", Guid.NewGuid(), "png", "hash", true),
            LocalPathBuilder.HomeSuccessStoryImage(Guid.NewGuid(), Guid.NewGuid(), "jpg", "hash", true),
            LocalPathBuilder.UserProfile(Guid.NewGuid(), "photo", Guid.NewGuid(), "png", "hash", true),
            LocalPathBuilder.JobReview(Guid.NewGuid(), Guid.NewGuid(), "png", "hash", true),
            LocalPathBuilder.InvitationAttachment(Guid.NewGuid(), Guid.NewGuid(), "png", "hash", true),
            "public/unicode/صورة 50%.png"
        ];
        foreach (var key in keys)
        {
            using var upload = new MemoryStream(Encoding.UTF8.GetBytes(PublicStorageFixture.PublicBody));
            Assert.That((await fixture.Storage.SaveAsync(upload, key)).IsSuccess, Is.True);
            var url = fixture.Storage.ToPublicUrl(key);
            Assert.That(url.IsSuccess, Is.True);
            Assert.That(StoragePathRules.TryPublicKey(url.Value[(fixture.Settings.PublicBaseUrl.Length + 1)..], out var decoded), Is.True);
            Assert.That(decoded, Is.EqualTo(key));
            var result = await new GetPublicFileHandler(fixture.Storage).Handle(new(decoded), default);
            Assert.That(result, Is.TypeOf<FileStreamResult>());
            var file = (FileStreamResult)result;
            await using var stream = file.FileStream;
            Assert.That(file.ContentType, Does.StartWith("image/"));
            Assert.That(file.EnableRangeProcessing, Is.True);
            Assert.That(file.LastModified, Is.Not.Null);
            Assert.That(file.EntityTag, Is.Not.Null);
            Assert.That(await new StreamReader(stream).ReadToEndAsync(), Is.EqualTo(PublicStorageFixture.PublicBody));
        }
    }

    [Test]
    public async Task PrivateQuestionImageUploadPreviewReplacementAndDeletionRemainAvailable()
    {
        using var fixture = new PublicStorageFixture();
        var key = LocalPathBuilder.QuestionImage(Guid.NewGuid(), Guid.NewGuid(), "png", "replacementhash");
        foreach (var content in new[] { "initial upload", "edited replacement" })
        {
            using var upload = new MemoryStream(Encoding.UTF8.GetBytes(content));
            Assert.That((await fixture.Storage.SaveAsync(upload, key)).IsSuccess, Is.True);
            Assert.That(await fixture.Storage.ExistsAsync(key, default), Is.True);
            Assert.That(fixture.Storage.MapPath(key, true).IsSuccess, Is.True);
            var file = await fixture.Storage.OpenReadAsync(key, default);
            Assert.That(file, Is.Not.Null);
            await using var stream = file!.Stream;
            Assert.That(await new StreamReader(stream).ReadToEndAsync(), Is.EqualTo(content));
            Assert.That(fixture.Storage.ToPublicUrl(key).IsFailed, Is.True);
        }
        Assert.That((await fixture.Storage.DeleteAsync(key)).IsSuccess, Is.True);
        Assert.That(await fixture.Storage.ExistsAsync(key, default), Is.False);
    }

    [TestCase("private/operation/question-bank/employee/images/image.png")]
    [TestCase("public/../private/operation/question-bank/employee/images/image.png")]
    [TestCase("public/%2e%2e/private/image.png")]
    public async Task PublicQueryCannotBypassRouteValidationEvenWithPermissiveStorage(string key)
    {
        var storage = new Mock<IFileStorageService>(MockBehavior.Strict);
        var result = await new GetPublicFileHandler(storage.Object).Handle(new GetPublicFileQuery(key), default);
        Assert.That(result, Is.TypeOf<NotFoundResult>());
        storage.VerifyNoOtherCalls();
    }

    [Test]
    public async Task UploadDoesNotFollowAPredictableTemporaryFileSymlink()
    {
        using var fixture = new PublicStorageFixture();
        var key = "public/new-image.png";
        var privateFile = Path.Combine(fixture.Root, fixture.PrivateKey);
        File.CreateSymbolicLink(Path.Combine(fixture.Root, key + ".tmp"), privateFile);
        using var body = new MemoryStream(Encoding.UTF8.GetBytes("new upload"));
        Assert.That((await fixture.Storage.SaveAsync(body, key)).IsSuccess, Is.True);
        Assert.That(File.ReadAllText(privateFile), Is.EqualTo(PublicStorageFixture.PrivateBody));
        Assert.That(File.ReadAllText(Path.Combine(fixture.Root, key)), Is.EqualTo("new upload"));
    }

    [TestCase(false)]
    [TestCase(true)]
    public async Task PublicFileAndDirectoryLinksCannotExposePrivateOrOutsideFiles(bool directoryLink)
    {
        using var fixture = new PublicStorageFixture();
        var privateFile = Path.Combine(fixture.Root, fixture.PrivateKey);
        foreach (var target in new[] { privateFile, Path.Combine(fixture.DirectoryPath, "storage-other", "outside.png") })
        {
            var alias = "alias" + Guid.NewGuid().ToString("N");
            var link = Path.Combine(fixture.Root, "public", alias);
            var key = "public/" + alias;
            if (directoryLink)
            {
                Directory.CreateSymbolicLink(link, Path.GetDirectoryName(target)!);
                key += "/" + Path.GetFileName(target);
            }
            else File.CreateSymbolicLink(link, target);
            Assert.That(fixture.Storage.MapPath(key).IsFailed, Is.True);
            Assert.That(await fixture.Storage.OpenReadAsync(key, default), Is.Null);
            Assert.That(fixture.Storage.ToPublicUrl(key).IsFailed, Is.True);
            Assert.That(await new GetPublicFileHandler(fixture.Storage).Handle(new(key), default), Is.TypeOf<NotFoundResult>());
        }
    }

    [Test]
    public async Task ALinkedPublicDirectoryOrBrokenFileLinkIsRejected()
    {
        using var fixture = new PublicStorageFixture();
        var broken = Path.Combine(fixture.Root, "public", "broken.png");
        File.CreateSymbolicLink(broken, Path.Combine(fixture.DirectoryPath, "missing.png"));
        Assert.That(fixture.Storage.MapPath("public/broken.png").IsFailed, Is.True);
        Directory.Delete(Path.Combine(fixture.Root, "public"), recursive: true);
        Directory.CreateSymbolicLink(Path.Combine(fixture.Root, "public"), Path.Combine(fixture.Root, "private"));
        var key = "public/" + fixture.PrivateKey["private/".Length..];
        Assert.That(fixture.Storage.MapPath(key).IsFailed, Is.True);
        Assert.That(await fixture.Storage.OpenReadAsync(key, default), Is.Null);
    }
}
