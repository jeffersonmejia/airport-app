using System.Security.Cryptography;
using System.Text;
using Airport.Features.Auth.Application.Ports;

namespace Airport.Features.Auth.Infrastructure.Security;

public sealed class LegacyMd5PasswordVerifier : IPasswordVerifier
{
    public bool Verify(string password, string storedHash)
    {
        if (storedHash.Length != 32)
        {
            return false;
        }

        try
        {
            var expectedHash = Convert.FromHexString(storedHash);
#pragma warning disable CA5351 // Compatibilidad temporal con los hashes del dump legado.
            var actualHash = MD5.HashData(Encoding.UTF8.GetBytes(password));
#pragma warning restore CA5351
            return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
