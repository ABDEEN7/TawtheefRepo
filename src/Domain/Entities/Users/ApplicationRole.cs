using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Tawtheef.Domain.Entities.Users;

public class ApplicationRole : IdentityRole<Guid>
{
    [MaxLength(50)]
    public string? NameAr { get; set; }
    [MaxLength(50)]
    public string? NameEn { get; set; }
    [MaxLength(150)]
    public string? DescriptionAr { get; set; }
    [MaxLength(150)]
    public string? DescriptionEn { get; set; }
}
