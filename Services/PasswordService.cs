using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using AuthX.Models;

namespace AuthX.Services;

public class PasswordService
{
    private readonly PasswordHasher<User> _hasher = new();

    public string HashPassword(User user, string password)
        => _hasher.HashPassword(user, password);

    public bool VerifyPassword(User user, string password)
        => _hasher.VerifyHashedPassword(user, user.PasswordHash, password)
           != PasswordVerificationResult.Failed;

    public static string GenerateResetToken()
    {
        return Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
    }
}