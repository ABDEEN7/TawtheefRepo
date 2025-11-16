using System.Linq.Expressions;
using CSharpFunctionalExtensions;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Common.Interfaces.Repositories;

public interface IUserRepository : IBaseRepository<User>
{
    Task<Result<User>> GetByEmailAsync(string email, params Expression<Func<User, object>>[]? includes);
    Task<Result<User>> GetByIdAsync(Guid id, params Expression<Func<User, object>>[]? includes);
    Task<Result<bool>> IsEmailUniqueAsync(string email);
    Task<Result<bool>> IsUserNameUniqueAsync(string userName);
    Task<Result<bool>> IsPhoneNumberUniqueAsync(string phoneNumber);
}
