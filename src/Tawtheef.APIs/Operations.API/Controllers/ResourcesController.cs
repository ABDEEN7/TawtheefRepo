using System.Text;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.WebUtilities;
using Tawtheef.Application.Common.Interfaces.Services.Resources;
using Tawtheef.Application.Features.Resources.DTOs;
using Tawtheef.Application.Features.Resources.Queries;
using Tawtheef.Infrastructure.Extensions;

namespace Operations.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ResourcesController(IMediator mediator) : ControllerBase
{
    private static readonly FileExtensionContentTypeProvider Mime = new();
    
    [HttpGet("{encoded}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public IActionResult Get(string encoded, [FromServices] IFileStorageService storage)
    {
        var blobKey = Decode(encoded);
        if (!blobKey.StartsWith("private/", StringComparison.OrdinalIgnoreCase))
            return BadRequest("Only private blob keys allowed here.");
    
        var map = storage.MapPath(blobKey, true);
        if (map.IsFailed) 
            return Problem(map.Errors[0].Message);
    
        var path = map.Value!;
        if (!System.IO.File.Exists(path)) return NotFound();
    
        var contentType = Mime.TryGetContentType(path, out var mt) ? mt : "application/octet-stream";
        var dlName = Path.GetFileName(path);
        return PhysicalFile(path, contentType, fileDownloadName: dlName, enableRangeProcessing: true);
    }
    
    [HttpGet("dl")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> GetSigned([FromQuery] string b, [FromQuery] long exp, [FromQuery] string sig, [FromServices] IFileStorageService storage)
    {
        var result = await mediator.Send(new GetSignedBlobQuery(b, exp, sig));
        if (result.IsFailed)
            return result.ToActionResult();

        var file = result.Value;

        if (file.SourceKind == FileSourceKind.RedirectUrl)
        {
            var blobKey = SafeDecode(b);
            var storedFile = await storage.OpenReadAsync(blobKey, HttpContext.RequestAborted);
            if (storedFile == null) return NotFound();

            // Set Content-Disposition to inline to allow browser preview, but keep filename
            var cd = new Microsoft.Net.Http.Headers.ContentDispositionHeaderValue("inline");
            cd.SetHttpFileName(file.DownloadName);
            Response.Headers.Append(Microsoft.Net.Http.Headers.HeaderNames.ContentDisposition, cd.ToString());

            return File(storedFile.Stream, file.ContentType);
        }

        // LocalPath
        var path = file.LocalPath!;
        if (string.IsNullOrWhiteSpace(path) || !Path.IsPathRooted(path) || !System.IO.File.Exists(path))
            return NotFound();

        var downloadName = Path.GetFileName(file.DownloadName).Replace("\r", "").Replace("\n", "").Trim();
        var contentType = string.IsNullOrWhiteSpace(file.ContentType) ? "application/octet-stream" : file.ContentType;

        return PhysicalFile(path, contentType, fileDownloadName: downloadName, enableRangeProcessing: file.EnableRangeProcessing);
    }
    
    [HttpGet("/files/{*path}")]
    public async Task<IActionResult> GetFile(string path)
    {
        var publicPath = "public/" + Uri.UnescapeDataString(path);
        return await mediator.Send(new GetPublicFileQuery(publicPath));
    }

    private static string SafeDecode(string encoded)
    {
        try
        {
            return Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(encoded));
        }
        catch
        {
            return encoded;
        }
    }
    private static string Decode(string encoded) => SafeDecode(encoded);
}

