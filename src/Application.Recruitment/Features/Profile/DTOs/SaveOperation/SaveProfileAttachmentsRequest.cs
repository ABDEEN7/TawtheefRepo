using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace Application.Recruitment.Features.Profile.DTOs.SaveOperation;

public sealed class SaveProfileAttachmentsRequest
{
    public bool Submit { get; set; }

    [RegularExpression(@"^[^\<\>]*$", ErrorMessage = "Invalid characters.")]
    public string BasicResourcesJson { get; set; } = string.Empty;

    [RegularExpression(@"^[^\<\>]*$", ErrorMessage = "Invalid characters.")]
    public string AttachmentsJson { get; set; } = string.Empty;

    [JsonIgnore]
    public List<IFormFile> AttachmentFiles { get; set; } = [];
}

public sealed class AdditionalAttachmentUpsertDto
{
    public Guid? Id { get; set; }

    [RegularExpression(@"^[a-zA-Z0-9\u0600-\u06FF\s\,\.\-]+$", ErrorMessage = "Title contains invalid characters.")]
    public string Title { get; set; } = default!;

    [RegularExpression(@"^[a-zA-Z0-9\s\,\.\-]+$", ErrorMessage = "File name contains invalid characters.")]
    public string FileName { get; set; } = default!;
    public Guid? AttachmentId { get; set; }
    public int? FileIndex { get; set; }
}

