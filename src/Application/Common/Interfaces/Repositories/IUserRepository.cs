using System.Linq.Expressions;
using FluentResults;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Common.Interfaces.Repositories;

public interface IUserRepository : IBaseRepository<User>
{
    Task<IResult<User>> GetByEmailAsync(string email, params Expression<Func<User, object>>[]? includes);
    Task<IResult<User>> GetByIdAsync(Guid id, params Expression<Func<User, object>>[]? includes);
    Task<IResult<bool>> IsEmailUniqueAsync(string email);
    Task<IResult<bool>> IsUserNameUniqueAsync(string userName);
    Task<IResult<bool>> IsPhoneNumberUniqueAsync(string phoneNumber);
}
