#region

using System.Security.Cryptography;

#endregion

namespace Share.DbContracts;

public static class SecurePasswordHasher {
	/// <summary>
	///     Size of salt.
	/// </summary>
	private const int SaltSize = 16;

	/// <summary>
	///     Size of hash.
	/// </summary>
	private const int HashSize = 20;

	/// <summary>
	///     Creates a hash from a password.
	/// </summary>
	/// <param name="password">The password.</param>
	/// <param name="iterations">Number of iterations.</param>
	/// <returns>The hash.</returns>
	public static string Hash(string password, int iterations = 1000)
    {
        // Create salt
        var salt = RandomNumberGenerator.GetBytes(SaltSize);

        // Create hash
        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations);
        var hash = pbkdf2.GetBytes(HashSize);

        // Combine salt and hash
        var hashBytes = new byte[SaltSize + HashSize];
        Array.Copy(salt, 0, hashBytes, 0, SaltSize);
        Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

        // Convert to base64
        return Convert.ToBase64String(hashBytes);
    }

	/// <summary>
	///     Verifies a password against a hash.
	/// </summary>
	/// <param name="password">The password.</param>
	/// <param name="hashedPassword">The hash.</param>
	/// <param name="iterations"></param>
	/// <returns>Could be verified?</returns>
	public static bool Verify(string password, string hashedPassword, int iterations = 1000)
    {
        // Get hash bytes
        var hashBytes = Convert.FromBase64String(hashedPassword);

        // Get salt
        var salt = new byte[SaltSize];
        Array.Copy(hashBytes, 0, salt, 0, SaltSize);

        // Create hash with given salt
        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations);
        var hash = pbkdf2.GetBytes(HashSize);

        // Get result
        for (var i = 0; i < HashSize; i++)
            if (hashBytes[i + SaltSize] != hash[i])
                return false;

        return true;
    }
}