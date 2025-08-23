using System.Collections.Concurrent;
using UserManagement.Services.Interfaces;

namespace UserManagement.Services.Implementations;
/// <summary>
/// An in-memory service for blacklisting JWT tokens.
/// </summary>
public class TokenBlacklistService : ITokenBlacklistService
{
    // ConcurrentDictionary is used for thread-safe operations on the blacklist.
    private readonly ConcurrentDictionary<string, bool> _blacklistedTokens = new();

    /// <summary>
    /// Adds a JWT token's unique identifier (JTI) to the blacklist.
    /// </summary>
    /// <param name="jti">The unique ID of the token to blacklist.</param>
    public void BlacklistToken(string jti)
    {
        _blacklistedTokens.TryAdd(jti, true);
    }

    /// <summary>
    /// Checks if a JWT token's unique identifier (JTI) is on the blacklist.
    /// </summary>
    /// <param name="jti">The unique ID of the token to check.</param>
    /// <returns>True if the token is blacklisted, otherwise false.</returns>
    public bool IsTokenBlacklisted(string jti)
    {
        return _blacklistedTokens.ContainsKey(jti);
    }
}
