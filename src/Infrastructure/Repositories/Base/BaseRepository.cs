using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Common.Interfaces;

namespace Tawtheef.Infrastructure.Repositories.Base;

public class BaseRepository<T>(IGenericRepository<T> repository) where T : class, IBaseEntity
{
    
    public IGenericRepository<T> Repository => repository ?? throw new ArgumentNullException(nameof(repository));
}