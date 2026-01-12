using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Common.Interfaces.Services.Security;

public interface IPasswordVerifier
{
    bool Verify(User user, string plainPassword);
}