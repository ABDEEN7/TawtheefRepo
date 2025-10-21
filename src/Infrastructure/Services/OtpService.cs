using System;
using System.Security.Cryptography;
using Tawtheef.Application.Common.Interfaces.Services;

namespace Tawtheef.Infrastructure.Services;

public class OtpService(TimeProvider time) : IOtpService
{
    private const int OtpLength = 4;
    private readonly TimeSpan _otpValidityDuration = TimeSpan.FromMinutes(10);
    private readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();

    public string GenerateOtp()
    {
        // Generate a cryptographically secure random number
        var randomNumber = new byte[4];
        _rng.GetBytes(randomNumber);
        var numericOtp = BitConverter.ToUInt32(randomNumber, 0) % (uint)Math.Pow(10, OtpLength);

        // Format as string with leading zeros
        return numericOtp.ToString($"D{OtpLength}");
    }

    public bool ValidateOtp(string otp, string storedOtp, DateTime expiryTime)
    {
        // Basic null checks
        if (string.IsNullOrWhiteSpace(otp)) return false;
        if (string.IsNullOrWhiteSpace(storedOtp)) return false;

        // Check if OTP is expired
        return time.GetLocalNow().DateTime <= expiryTime &&
               // Constant-time comparison to prevent timing attacks
               CryptographicEquals(otp, storedOtp);
    }

    private static bool CryptographicEquals(string a, string b)
    {
        if (a.Length != b.Length)
        {
            return false;
        }

        var result = 0;
        for (var i = 0; i < a.Length; i++)
        {
            result |= a[i] ^ b[i];
        }

        return result == 0;
    }

    public TimeSpan GetOtpValidityDuration() => _otpValidityDuration;

    public void Dispose()
    {
        _rng.Dispose();
    }
}