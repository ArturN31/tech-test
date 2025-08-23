namespace UserManagement.Services.Interfaces;

/// <summary>
/// Defines a service for blacklisting JWT tokens.
/// </summary>
public interface ITokenBlacklistService
{
    /// <summary>
    /// Adds a JWT token's unique identifier (JTI) to the blacklist.
    /// </summary>
    /// <param name="jti">The unique ID of the token to blacklist.</param>
    void BlacklistToken(string jti);

    /// <summary>
    /// Checks if a JWT token's unique identifier (JTI) is on the blacklist.
    /// </summary>
    /// <param name="jti">The unique ID of the token to check.</param>
    /// <returns>True if the token is blacklisted, otherwise false.</returns>
    bool IsTokenBlacklisted(string jti);
}
