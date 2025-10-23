using System;

namespace Tawtheef.Application.Common.Interfaces.Services;

public interface IOtpService
{
    string GenerateOtp();
    bool ValidateOtp(string otp, string storedOtp, DateTime expiryTime);
}