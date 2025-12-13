using Microsoft.AspNetCore.Identity;

namespace Tawtheef.Domain.Entities.Users;

public class ApplicationRole : IdentityRole<Guid>
{
    public string? NameAr { get; set; }
    public string? NameEn { get; set; }
    public string? DescriptionAr { get; set; }
    public string? DescriptionEn { get; set; }

    public ApplicationRole() : base()
    {
    }

    public ApplicationRole(string roleName) : base(roleName)
    {
    }
}
