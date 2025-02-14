using Microsoft.AspNetCore.Identity;

public class PasswordHasherHelper
{
    private readonly PasswordHasher<string> _hasher = new PasswordHasher<string>();

    public string HashPassword(string password)
    {
        return _hasher.HashPassword("defaultUser", password); // ✅ FIXED
    }

    public bool VerifyPassword(string hashedPassword, string providedPassword)
    {
        return _hasher.VerifyHashedPassword("defaultUser", hashedPassword, providedPassword)
               == PasswordVerificationResult.Success; // ✅ FIXED
    }
}
