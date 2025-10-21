using System;

namespace Tawtheef.Domain.Common.Interfaces;

public interface ITrackedEntity : IEntity
{
    Guid? CreatedById { get; set; }
    DateTimeOffset CreatedDate { get; set; }
    Guid? UpdatedById { get; set; }
    DateTimeOffset? UpdatedDate { get; set; }
}
