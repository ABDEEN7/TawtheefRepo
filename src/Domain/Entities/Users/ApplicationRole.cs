using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Tawtheef.Domain.Entities.Users;

public static class ApplicationRoleIds
{
    public static Guid SystemAdmin = Guid.Parse("1361d691-53c5-4a84-aea1-64ff134cf082");
    public static Guid HRAdmin = Guid.Parse("5f12e420-f666-4af4-a8fa-4e4aa755fdcd");
    public static Guid OfficeAdmin = Guid.Parse("98e20970-b6bc-4da9-a947-f75e9adae3ca");
    public static Guid OfficeUser = Guid.Parse("12f5805d-6970-4a9e-a275-2b7cf3db3bb8");
}
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
    public bool IsSystemRole { get; set; } = false;
}
