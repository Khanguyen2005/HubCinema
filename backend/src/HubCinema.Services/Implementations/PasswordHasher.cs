using HubCinema.Services.Interfaces;

namespace HubCinema.Services.Implementations;

/// <summary>BCrypt password hashing. The salt and work factor are embedded in the hash string.</summary>
public class PasswordHasher : IPasswordHasher
{
    public string Hash(string password) => BCrypt.Net.BCrypt.HashPassword(password);

    public bool Verify(string password, string passwordHash) => BCrypt.Net.BCrypt.Verify(password, passwordHash);
}
