using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Tawtheef.Domain.Entities.Users;

public class OfficeUser : User
{
    public Guid OfficeId { get; set; }
    public required Office Office { get; set; }
}
