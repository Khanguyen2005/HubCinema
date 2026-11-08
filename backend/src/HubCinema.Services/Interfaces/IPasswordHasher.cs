namespace HubCinema.Services.Interfaces;

/// <summary>Hashes and verifies passwords. The algorithm/work factor is an implementation detail.</summary>
public interface IPasswordHasher
{
    string Hash(string password);

    bool Verify(string password, string passwordHash);
}
