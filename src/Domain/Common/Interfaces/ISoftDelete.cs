using System;

namespace Tawtheef.Domain.Common.Interfaces;

public interface ISoftDelete
{
    bool IsDeleted { get; set; }
    Guid? DeletedById { get; set; }
    DateTimeOffset? DeletedDate { get; set; }
}
