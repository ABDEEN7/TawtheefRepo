using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities;

[Index(nameof(Key),IsUnique = true)]
public class Resource : EventEntity
{
    [Required, StringLength(200)]
    public required string Name { get; init; }
    
    [StringLength(1000)]
    public string? Description { get; init; }
    
    [Required, StringLength(2048)]
    public required string Url { get; init; }
    
    [Required, StringLength(36)]
    public required string Key { get; init; } = Guid.NewGuid().ToString();
    
    [Required, StringLength(50)]
    public required string Type { get; set; }
    public ulong Size { get; init; }
    
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? AdditionalData { get; set; }
}
