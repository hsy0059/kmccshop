using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Campus.Common.Security;

public static class PasswordHasher
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 100000;
    private const KeyDerivationPrf Prf = KeyDerivationPrf.HMACSHA256;

    public static string HashPassword(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
        byte[] hash = KeyDerivation.Pbkdf2(password, salt, Prf, Iterations, HashSize);

        byte[] result = new byte[SaltSize + HashSize + 4];
        Buffer.BlockCopy(salt, 0, result, 0, SaltSize);
        Buffer.BlockCopy(hash, 0, result, SaltSize, HashSize);
        BitConverter.GetBytes(Iterations).CopyTo(result, SaltSize + HashSize);

        return Convert.ToBase64String(result);
    }

    public static bool VerifyPassword(string password, string hashedPassword)
    {
        try
        {
            byte[] decoded = Convert.FromBase64String(hashedPassword);

            if (decoded.Length < SaltSize + HashSize + 4)
                return false;

            byte[] salt = new byte[SaltSize];
            byte[] storedHash = new byte[HashSize];

            Buffer.BlockCopy(decoded, 0, salt, 0, SaltSize);
            Buffer.BlockCopy(decoded, SaltSize, storedHash, 0, HashSize);

            byte[] hash = KeyDerivation.Pbkdf2(password, salt, Prf, Iterations, HashSize);

            return CryptographicOperations.FixedTimeEquals(storedHash, hash);
        }
        catch
        {
            return false;
        }
    }
}
