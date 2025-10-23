using System.Threading.Tasks;
using Tawtheef.Application.Common.Models;

namespace Tawtheef.Application.Common.Interfaces.Services;

public interface IRecaptchaService
{
    Task<RecaptchaResponse> Verify(string recaptchaToken);
}
