using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Domain.Configurations.Settings;

namespace Tawtheef.Infrastructure.Services.Security;

public sealed class AccessCodeProtector : IAccessCodeProtector
{
    private const string Purpose = "Tawtheef.TestSlots.AccessCode.v1";
    private const string PayloadVersion = "v1";
    private const int KeyLength = 32;
    private const int NonceLength = 12;
    private const int TagLength = 16;

    private readonly byte[] encryptionKey;

    public AccessCodeProtector(IOptions<TestSlotAccessCodeOptions> options)
    {
        encryptionKey = DecodeEncryptionKey(options.Value.EncryptionKey);
    }

    public string Protect(string accessCode)
    {
        ArgumentNullException.ThrowIfNull(accessCode);

        var plaintext = Encoding.UTF8.GetBytes(accessCode);
        var nonce = RandomNumberGenerator.GetBytes(NonceLength);
        var ciphertext = new byte[plaintext.Length];
        var tag = new byte[TagLength];

        using (var aesGcm = new AesGcm(encryptionKey, TagLength))
        {
            aesGcm.Encrypt(nonce, plaintext, ciphertext, tag, Encoding.UTF8.GetBytes(Purpose));
        }

        return string.Join('.', PayloadVersion, Convert.ToBase64String(nonce), Convert.ToBase64String(ciphertext),
            Convert.ToBase64String(tag));
    }

    public string Unprotect(string protectedAccessCode)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(protectedAccessCode);

        try
        {
            var parts = protectedAccessCode.Split('.', StringSplitOptions.None);
            if (parts.Length != 4 || parts[0] != PayloadVersion || parts.Skip(1).Any(string.IsNullOrEmpty))
                throw new CryptographicException("Invalid TestSlot Access Code payload.");

            var nonce = Convert.FromBase64String(parts[1]);
            var ciphertext = Convert.FromBase64String(parts[2]);
            var tag = Convert.FromBase64String(parts[3]);
            if (nonce.Length != NonceLength || tag.Length != TagLength || ciphertext.Length == 0)
                throw new CryptographicException("Invalid TestSlot Access Code payload.");

            var plaintext = new byte[ciphertext.Length];
            using (var aesGcm = new AesGcm(encryptionKey, TagLength))
            {
                aesGcm.Decrypt(nonce, ciphertext, tag, plaintext, Encoding.UTF8.GetBytes(Purpose));
            }

            return Encoding.UTF8.GetString(plaintext);
        }
        catch (FormatException exception)
        {
            throw new CryptographicException("Invalid TestSlot Access Code payload.", exception);
        }
    }

    internal static byte[] DecodeEncryptionKey(string? encryptionKey)
    {
        if (string.IsNullOrWhiteSpace(encryptionKey))
            throw new InvalidOperationException("TestSlotAccessCode:EncryptionKey is required.");

        try
        {
            var key = Convert.FromBase64String(encryptionKey);
            if (key.Length != KeyLength)
                throw new InvalidOperationException("TestSlotAccessCode:EncryptionKey must decode to exactly 32 bytes.");

            return key;
        }
        catch (FormatException exception)
        {
            throw new InvalidOperationException("TestSlotAccessCode:EncryptionKey must be valid Base64.", exception);
        }
    }

}
