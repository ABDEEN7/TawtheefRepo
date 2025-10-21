using Tawtheef.Domain.Common.Interfaces;

namespace Tawtheef.Application.Common.Interfaces.Repositories.Base;

public interface IBaseRepository<T> where T : class, IBaseEntity
{
    // ReSharper disable once UnusedMember.Global
    IGenericRepository<T> Repository { get; }

}
