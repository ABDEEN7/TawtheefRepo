using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;

namespace Tawtheef.Application.Common.Interfaces.Services;

public interface IVerificationService
{
    Task LogVerificationAttempt(string email);
    bool CanResendEmail(string email);
    int GetResendCooldown(string email);
}
