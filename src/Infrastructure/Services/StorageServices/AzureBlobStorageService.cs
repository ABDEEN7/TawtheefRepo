using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Blobs;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Options;
using Serilog;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Domain.Configurations;
using Tawtheef.Domain.Constants;

namespace Tawtheef.Infrastructure.Services.StorageServices;

    public sealed class AzureBlobStorageService : IFileStorageService
    {
        private readonly BlobContainerClient _container;
        private readonly string? _publicBaseUrl;
        private readonly ILogger _logger;

        public AzureBlobStorageService(
            BlobServiceClient blobServiceClient,
            IOptions<StorageSettings> storageSettings,
            ILogger logger)
        {
            var cfg = storageSettings.Value ?? throw new InvalidOperationException("Storage settings missing");

            string containerName = cfg.RootPath ?? throw new InvalidOperationException("Storage:ContainerName missing");
            _publicBaseUrl = string.IsNullOrWhiteSpace(cfg.PublicBaseUrl) ? null : cfg.PublicBaseUrl.TrimEnd('/');

            _container = blobServiceClient.GetBlobContainerClient(containerName);
            _logger = logger.ForContext<AzureBlobStorageService>();
        }

        public async Task<Result<FileSaved>> SaveAsync(Stream stream, string blobKey, CancellationToken ct = default)
        {
            try
            {
                ct.ThrowIfCancellationRequested();

                if (string.IsNullOrWhiteSpace(blobKey))
                    return Result.Failure<FileSaved>(ErrorsCodes.InvalidBlobKey);

                // normalize blob key to use forward slashes
                blobKey = blobKey.Replace('\\', '/');

                var blobClient = _container.GetBlobClient(blobKey);

                // Upload (will overwrite)
                // Reset stream position if possible
                if (stream.CanSeek)
                {
                    try { stream.Position = 0; } catch { /* ignore */ }
                }

                await blobClient.UploadAsync(stream, overwrite: true, cancellationToken: ct).ConfigureAwait(false);

                // Fetch properties to get size
                var props = await blobClient.GetPropertiesAsync(cancellationToken: ct).ConfigureAwait(false);
                var size = props.Value.ContentLength;

                return Result.Success(new FileSaved(blobKey, size));
            }
            catch (OperationCanceledException oce)
            {
                _logger.Error(oce, "SaveAsync cancelled for {BlobKey}", blobKey);
                return Result.Failure<FileSaved>(ErrorsCodes.Cancelled);
            }
            catch (RequestFailedException rfe) when (rfe.Status == 403)
            {
                _logger.Error(rfe, "SaveAsync access denied for {BlobKey}", blobKey);
                return Result.Failure<FileSaved>(ErrorsCodes.AccessDenied);
            }
            catch (RequestFailedException rfe)
            {
                _logger.Error(rfe, "SaveAsync Azure error for {BlobKey}", blobKey);
                return Result.Failure<FileSaved>(ErrorsCodes.IoError);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "SaveAsync error for {BlobKey}", blobKey);
                return Result.Failure<FileSaved>(ErrorsCodes.IoError);
            }
        }

        public async Task<Result<bool>> DeleteAsync(string blobKey, CancellationToken ct = default)
        {
            try
            {
                ct.ThrowIfCancellationRequested();

                if (string.IsNullOrWhiteSpace(blobKey))
                    return Result.Failure<bool>(ErrorsCodes.InvalidBlobKey);

                blobKey = blobKey.Replace('\\', '/');

                var blobClient = _container.GetBlobClient(blobKey);
                var resp = await blobClient.DeleteIfExistsAsync(cancellationToken: ct).ConfigureAwait(false);

                return Result.Success(resp.Value);
            }
            catch (OperationCanceledException oce)
            {
                _logger.Error(oce, "DeleteAsync cancelled for {BlobKey}", blobKey);
                return Result.Failure<bool>(ErrorsCodes.Cancelled);
            }
            catch (RequestFailedException rfe) when (rfe.Status == 403)
            {
                _logger.Error(rfe, "DeleteAsync access denied for {BlobKey}", blobKey);
                return Result.Failure<bool>(ErrorsCodes.AccessDenied);
            }
            catch (RequestFailedException rfe)
            {
                _logger.Error(rfe, "DeleteAsync Azure error for {BlobKey}", blobKey);
                return Result.Failure<bool>(ErrorsCodes.IoError);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DeleteAsync error for {BlobKey}", blobKey);
                return Result.Failure<bool>(ErrorsCodes.IoError);
            }
        }

        public async Task<Result<int>> DeletePrefixAsync(string prefix, CancellationToken ct = default)
        {
            try
            {
                ct.ThrowIfCancellationRequested();

                if (string.IsNullOrWhiteSpace(prefix))
                    return Result.Failure<int>(ErrorsCodes.InvalidBlobKey);

                // normalize
                prefix = prefix.Replace('\\', '/');

                var deleted = 0;

                // Use pagination to iterate blobs with prefix
                await foreach (var blobItem in _container.GetBlobsAsync(prefix: prefix, cancellationToken: ct).ConfigureAwait(false))
                {
                    ct.ThrowIfCancellationRequested();

                    try
                    {
                        var client = _container.GetBlobClient(blobItem.Name);
                        var delResp = await client.DeleteIfExistsAsync(cancellationToken: ct).ConfigureAwait(false);
                        if (delResp.Value) deleted++;
                    }
                    catch (RequestFailedException rf)
                    {
                        // log and continue (best-effort)
                        _logger.Warning(rf, "Failed to delete blob {Blob} while deleting prefix {Prefix}", blobItem.Name, prefix);
                    }
                }

                return Result.Success(deleted);
            }
            catch (OperationCanceledException oce)
            {
                _logger.Error(oce, "DeletePrefixAsync cancelled for {Prefix}", prefix);
                return Result.Failure<int>(ErrorsCodes.Cancelled);
            }
            catch (RequestFailedException rfe) when (rfe.Status == 403)
            {
                _logger.Error(rfe, "DeletePrefixAsync access denied for {Prefix}", prefix);
                return Result.Failure<int>(ErrorsCodes.AccessDenied);
            }
            catch (RequestFailedException rfe)
            {
                _logger.Error(rfe, "DeletePrefixAsync Azure error for {Prefix}", prefix);
                return Result.Failure<int>(ErrorsCodes.IoError);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DeletePrefixAsync error for {Prefix}", prefix);
                return Result.Failure<int>(ErrorsCodes.IoError);
            }
        }

        public Result<string> ToPublicUrl(string blobKey)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_publicBaseUrl))
                    return Result.Failure<string>(ErrorsCodes.ConfigMissing);

                if (string.IsNullOrWhiteSpace(blobKey))
                    return Result.Failure<string>(ErrorsCodes.InvalidBlobKey);

                // follow same rule as LocalStorageService: require public/ prefix
                const string pubPrefix = "public/";
                if (!blobKey.StartsWith(pubPrefix, StringComparison.OrdinalIgnoreCase))
                    return Result.Failure<string>(ErrorsCodes.NotPublicResource);

                var relative = blobKey[pubPrefix.Length..].Replace('\\', '/');
                var url = $"{_publicBaseUrl}/{relative}";
                return Result.Success(url);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "ToPublicUrl error for {BlobKey}", blobKey);
                return Result.Failure<string>(ErrorsCodes.IoError);
            }
        }

        public Result<string> MapPath(string blobKey)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(blobKey))
                    return Result.Failure<string>(ErrorsCodes.InvalidBlobKey);

                // Normalize
                blobKey = blobKey.Replace('\\', '/');

                // Return the absolute URI of the blob as the "path" equivalent.
                var blobClient = _container.GetBlobClient(blobKey);
                var uri = blobClient.Uri.AbsoluteUri;

                return Result.Success(uri);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "MapPath error for {BlobKey}", blobKey);
                return Result.Failure<string>(ErrorsCodes.InvalidBlobKey);
            }
        }
    }
