using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
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

        // IMPORTANT: container name must be lowercase and must NOT contain '/'
        var containerName = cfg.RootPath ?? throw new InvalidOperationException("Storage:ContainerName missing");
        containerName = containerName.Trim();

        if (containerName.Contains('/'))
            throw new InvalidOperationException("Storage:ContainerName must not contain '/'");

        containerName = containerName.ToLowerInvariant();

        _publicBaseUrl = string.IsNullOrWhiteSpace(cfg.PublicBaseUrl) ? null : cfg.PublicBaseUrl.TrimEnd('/');

        _container = blobServiceClient.GetBlobContainerClient(containerName);
        _logger = logger.ForContext(typeof(AzureBlobStorageService));
    }

    private string BuildBlobName(string blobKey)
    {
        blobKey = blobKey.Trim().Replace('\\', '/').TrimStart('/');
        return blobKey;
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
                try { stream.Position = 0; }
                catch
                {
                    // ignored
                }
            }

            // Optional but recommended: ensure container exists (especially in Stage/Dev)
            await _container.CreateIfNotExistsAsync(cancellationToken: ct).ConfigureAwait(false);

            await blobClient.UploadAsync(
                stream,
                new BlobUploadOptions
                {
                    HttpHeaders = new BlobHttpHeaders
                    {
                        // if you know file type, set it; otherwise omit
                        // ContentType = "application/pdf"
                    }
                },
                ct
            ).ConfigureAwait(false);

            var props = await blobClient.GetPropertiesAsync(cancellationToken: ct).ConfigureAwait(false);
            var size = props.Value.ContentLength;

            return Result.Ok(new FileSaved(blobName, (ulong)size));
        }
        catch (RequestFailedException rfe)
        {
            // This will tell you EXACT reason (InvalidResourceName, ContainerNotFound, etc.)
            _logger.Error(rfe, "SaveAsync Azure error. Status={Status} Code={Code} Msg={Msg}",
                rfe.Status, rfe.ErrorCode, rfe.Message);

            if (rfe.Status == 403) return Result.Fail<FileSaved>(ErrorsCodes.AccessDenied);
            if (rfe.Status == 404) return Result.Fail<FileSaved>(ErrorsCodes.NotFound);

            return Result.Fail<FileSaved>(ErrorsCodes.IoError);
        }
        catch (OperationCanceledException oce)
        {
            _logger.Information(oce, "SaveAsync cancelled for {BlobKey}", blobKey);
            return Result.Fail<FileSaved>(ErrorsCodes.Cancelled);
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

                blobKey = blobKey.Replace('\\', '/');

                var blobClient = _container.GetBlobClient(blobKey);
                var resp = await blobClient.DeleteIfExistsAsync(cancellationToken: ct).ConfigureAwait(false);

                return Result.Ok(resp.Value);
            }
            catch (OperationCanceledException oce)
            {
                _logger.Information(oce, "DeleteAsync cancelled for {BlobKey}", blobKey);
                return Result.Fail<bool>(ErrorsCodes.Cancelled);
            }
            catch (RequestFailedException rfe) when (rfe.Status == 403)
            {
                _logger.Error(rfe, "DeleteAsync access denied for {BlobKey}", blobKey);
                return Result.Fail<bool>(ErrorsCodes.AccessDenied);
            }
            catch (RequestFailedException rfe)
            {
                _logger.Error(rfe, "DeleteAsync Azure error for {BlobKey}", blobKey);
                return Result.Fail<bool>(ErrorsCodes.IoError);
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

                // follow same rule as LocalStorageService: require public/ prefix
                const string pubPrefix = "public/";
                if (!blobKey.StartsWith(pubPrefix, StringComparison.OrdinalIgnoreCase))
                    return Result.Fail<string>(ErrorsCodes.NotPublicResource);

                var relative = blobKey[pubPrefix.Length..].Replace('\\', '/');
                var url = $"{_publicBaseUrl}/{relative}";
                return Result.Ok(url);
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

                // Normalize
                blobKey = blobKey.Replace('\\', '/');

                // Return the absolute URI of the blob as the "path" equivalent.
                var blobClient = _container.GetBlobClient(blobKey);
                var uri = blobClient.Uri.AbsoluteUri;

                return Result.Ok(uri);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "MapPath error for {BlobKey}", blobKey);
                return Result.Fail<string>(ErrorsCodes.InvalidBlobKey);
            }
        }
        
        public async Task<bool> ExistsAsync(string path, CancellationToken ct)
        {
            var blob = _container.GetBlobClient(Norm(path));
            return await blob.ExistsAsync(ct);
        }

        public async Task<StoredFileStream?> OpenReadAsync(string path, CancellationToken ct)
        {
            var blob = _container.GetBlobClient(Norm(path));
            if (!await blob.ExistsAsync(ct)) return null;

            var resp = await blob.DownloadStreamingAsync(cancellationToken: ct);
            var d = resp.Value.Details;

            return new StoredFileStream(
                Stream: resp.Value.Content,
                ContentType: d.ContentType ?? "application/octet-stream",
                ETag: d.ETag.ToString(),
                LastModified: d.LastModified,
                Length: d.ContentLength
            );
        }

        private static string Norm(string path) => path.Replace('\\', '/').TrimStart('/');
}
