using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;

namespace Tawtheef.Application.Features.Recruitment.Profile.DTOs;

public sealed class SaveProfileAttachmentsRequest
{
    public bool Submit { get; set; }

    public string AttachmentsJson { get; set; } = string.Empty;

    [JsonIgnore]
    public List<IFormFile> AttachmentFiles { get; set; } = [];
}

public sealed class AdditionalAttachmentUpsertDto
{
    public Guid? Id { get; set; }
    public string Title { get; set; } = default!;
    public string FileName { get; set; } = default!;
    public Guid? AttachmentId { get; set; }
    public int? FileIndex { get; set; }
}
