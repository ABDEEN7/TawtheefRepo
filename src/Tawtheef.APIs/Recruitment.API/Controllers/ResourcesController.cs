using System.Text;
using Cortex.Mediator;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.WebUtilities;
using Tawtheef.Application.Common;
using Tawtheef.Application.Common.Interfaces.Services.Resources;
using Tawtheef.Application.Features.Resources.DTOs;
using Tawtheef.Application.Features.Resources.Queries;
using Tawtheef.Infrastructure.Extensions;

namespace Recruitment.API.Controllers;

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
    public async Task<IActionResult> GetSigned([FromQuery] string b, [FromQuery] long exp, [FromQuery] string sig)
    {
        var result = await mediator.Send(new GetSignedBlobQuery(b, exp, sig));
        if (result.IsFailed)
            return result.ToActionResult();

        var file = result.Value;

        if (file.SourceKind == FileSourceKind.RedirectUrl)
            return Redirect(file.RedirectUrl!);

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
        return await mediator.Send(new GetPublicFileQuery(Uri.UnescapeDataString(path)));
    }
    private static string Decode(string encoded)
        => Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(encoded));
}
