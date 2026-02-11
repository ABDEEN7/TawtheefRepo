namespace Tawtheef.Domain.Common.Interfaces;

public interface ISoftDelete
{
    bool IsDeleted { get; set; }
    Guid? DeletedById { get; set; }
    DateTime? DeletedDate { get; set; }
}
