using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using FluentResults;
using Microsoft.Extensions.Options;
using Tawtheef.Application.Common.Interfaces.Logging;
using Tawtheef.Application.Common.Interfaces.Services.Resources;
using Tawtheef.Domain.Configurations.Settings;
using Tawtheef.Domain.Constants;

namespace Tawtheef.Infrastructure.Services.StorageServices;

public sealed class AzureBlobStorageService : IFileStorageService
{
    private readonly BlobContainerClient _container;
    private readonly string? _publicBaseUrl;
    private readonly IAppLogger _logger;

    public AzureBlobStorageService(
        BlobServiceClient blobServiceClient,
        IOptions<StorageSettings> storageSettings,
        IAppLogger logger)
    {
        var cfg = storageSettings.Value ?? throw new InvalidOperationException("Storage settings missing");

        var containerName = cfg.RootPath ?? throw new InvalidOperationException("Storage:ContainerName missing");
        containerName = containerName.Trim();

        if (containerName.Contains('/'))
            throw new InvalidOperationException("Storage:ContainerName must not contain '/'");

        containerName = containerName.ToLowerInvariant();

        _publicBaseUrl = string.IsNullOrWhiteSpace(cfg.PublicBaseUrl) ? null : cfg.PublicBaseUrl.TrimEnd('/');

        _container = blobServiceClient.GetBlobContainerClient(containerName);
        _logger = logger.ForContext(typeof(AzureBlobStorageService));
    }

    private static string BuildBlobName(string blobKey)
        => blobKey.Trim().Replace('\\', '/').TrimStart('/');

    public Task<IResult<string>> GetSignedReadUrlAsync(string blobKey, TimeSpan ttl, 
        string? downloadName = null, CancellationToken ct = default)
    {
        try
        {
            ct.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(blobKey))
                return Task.FromResult<IResult<string>>(Result.Fail<string>(ErrorsCodes.InvalidBlobKey));

            var blobName = BuildBlobName(blobKey);
            var blobClient = _container.GetBlobClient(blobName);

            if (!blobClient.CanGenerateSasUri)
            {
                _logger.Error("BlobClient cannot generate SAS URI. Ensure connection string includes shared key.");
                return Task.FromResult<IResult<string>>(Result.Fail<string>(ErrorsCodes.ConfigMissing));
            }

            var expiresOn = DateTimeOffset.UtcNow.Add(ttl);

            var sas = new BlobSasBuilder
            {
                BlobContainerName = _container.Name,
                BlobName = blobName,
                Resource = "b",
                ExpiresOn = expiresOn
            };
            sas.SetPermissions(BlobSasPermissions.Read);

            // Optional: force download name
            if (!string.IsNullOrWhiteSpace(downloadName))
            {
                downloadName = Path.GetFileName(downloadName).Replace("\r", "").Replace("\n", "").Trim();
                sas.ContentDisposition = $"attachment; filename=\"{downloadName}\"";
            }

            var sasUri = blobClient.GenerateSasUri(sas);
            return Task.FromResult<IResult<string>>(Result.Ok(sasUri.ToString()));
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "GetSignedReadUrlAsync error for {BlobKey}", blobKey);
            return Task.FromResult<IResult<string>>(Result.Fail<string>(ErrorsCodes.IoError));
        }
    }

    public async Task<IResult<FileSaved>> SaveAsync(Stream stream, string blobKey, CancellationToken ct = default)
    {
        try
        {
            ct.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(blobKey))
                return Result.Fail<FileSaved>(ErrorsCodes.InvalidBlobKey);

            var blobName = BuildBlobName(blobKey);
            var blobClient = _container.GetBlobClient(blobName);

            if (stream.CanSeek)
            {
                try { stream.Position = 0; } catch { /* ignore */ }
            }

            await _container.CreateIfNotExistsAsync(cancellationToken: ct).ConfigureAwait(false);

            await blobClient.UploadAsync(
                stream,
                new BlobUploadOptions
                {
                    HttpHeaders = new BlobHttpHeaders
                    {
                        // ContentType = "application/pdf" // set if you know it
                    }
                },
                ct).ConfigureAwait(false);

            var props = await blobClient.GetPropertiesAsync(cancellationToken: ct).ConfigureAwait(false);
            return Result.Ok(new FileSaved(blobName, (ulong)props.Value.ContentLength));
        }
        catch (RequestFailedException rfe)
        {
            _logger.Error(rfe, "SaveAsync Azure error. Status={Status} Code={Code} Msg={Msg}",
                rfe.Status, rfe.ErrorCode, rfe.Message);

            if (rfe.Status == 403) return Result.Fail<FileSaved>(ErrorsCodes.AccessDenied);
            if (rfe.Status == 404) return Result.Fail<FileSaved>(ErrorsCodes.NotFound);

            return Result.Fail<FileSaved>(ErrorsCodes.IoError);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "SaveAsync error for {BlobKey}", blobKey);
            return Result.Fail<FileSaved>(ErrorsCodes.IoError);
        }
    }

    public async Task<Result<bool>> DeleteAsync(string blobKey, CancellationToken ct = default)
    {
        try
        {
            ct.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(blobKey))
                return Result.Fail<bool>(ErrorsCodes.InvalidBlobKey);

            var blobName = BuildBlobName(blobKey);
            var blobClient = _container.GetBlobClient(blobName);

            var resp = await blobClient.DeleteIfExistsAsync(cancellationToken: ct).ConfigureAwait(false);
            return Result.Ok(resp.Value);
        }
        catch (RequestFailedException rfe) when (rfe.Status == 403)
        {
            _logger.Error(rfe, "DeleteAsync access denied for {BlobKey}", blobKey);
            return Result.Fail<bool>(ErrorsCodes.AccessDenied);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "DeleteAsync error for {BlobKey}", blobKey);
            return Result.Fail<bool>(ErrorsCodes.IoError);
        }
    }

    public IResult<string> ToPublicUrl(string blobKey)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(_publicBaseUrl))
                return Result.Fail<string>(ErrorsCodes.ConfigMissing);

            if (string.IsNullOrWhiteSpace(blobKey))
                return Result.Fail<string>(ErrorsCodes.InvalidBlobKey);

            const string pubPrefix = "public/";
            if (!blobKey.StartsWith(pubPrefix, StringComparison.OrdinalIgnoreCase))
                return Result.Fail<string>(ErrorsCodes.NotPublicResource);

            var relative = BuildBlobName(blobKey)[pubPrefix.Length..];
            return Result.Ok($"{_publicBaseUrl}/{Uri.EscapeDataString(relative)}");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "ToPublicUrl error for {BlobKey}", blobKey);
            return Result.Fail<string>(ErrorsCodes.IoError);
        }
    }

    public IResult<string> MapPath(string blobKey, bool isReadOperation = false)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(blobKey))
                return Result.Fail<string>(ErrorsCodes.InvalidBlobKey);

            // IMPORTANT: return normalized blob name (NOT URL)
            var blobName = BuildBlobName(blobKey);

            // optional: if read op, verify exists (might cost a call; skip if you want)
            // if (isReadOperation) { ... await ExistsAsync(blobName) ... } // but MapPath is sync, so skip

            return Result.Ok(blobName);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "MapPath error for {BlobKey}", blobKey);
            return Result.Fail<string>(ErrorsCodes.InvalidBlobKey);
        }
    }

    public async Task<bool> ExistsAsync(string pathOrBlobName, CancellationToken ct)
    {
        var blobName = BuildBlobName(pathOrBlobName);
        var blob = _container.GetBlobClient(blobName);
        return await blob.ExistsAsync(ct).ConfigureAwait(false);
    }

    public async Task<StoredFileStream?> OpenReadAsync(string pathOrBlobName, CancellationToken ct)
    {
        var path = Path.Combine("public", pathOrBlobName);
        var blobName = BuildBlobName(path);
        var blob = _container.GetBlobClient(blobName);

        if (!await blob.ExistsAsync(ct).ConfigureAwait(false))
        {
            _logger.Warning("Blob {BlobName} does not exist, received path: {PathOrBlobName}", blobName, pathOrBlobName);
            return null;
        }

        var resp = await blob.DownloadStreamingAsync(cancellationToken: ct).ConfigureAwait(false);
        var d = resp.Value.Details;

        return new StoredFileStream(
            Stream: resp.Value.Content,
            ContentType: d.ContentType ?? "application/octet-stream",
            ETag: d.ETag.ToString(),
            LastModified: d.LastModified,
            Length: d.ContentLength
        );
    }
}
