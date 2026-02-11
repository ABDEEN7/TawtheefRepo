namespace Tawtheef.Domain.Common.Interfaces;

public interface ITrackedEntity : IEntity
{
    Guid? CreatedById { get; set; }
    DateTime CreatedDate { get; set; }
    Guid? UpdatedById { get; set; }
    DateTime? UpdatedDate { get; set; }
}
