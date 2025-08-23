using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using UserManagement.Data.Models;
using UserManagement.Blazor.Models;
using UserManagement.Services.Interfaces;

namespace UserManagement.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountManagementController(
    UserManager<User> userManager,
    IConfiguration _configuration,
    ITokenBlacklistService blacklistService) : ControllerBase
{
    // Login endpoint to authenticate users and generate JWT tokens
    [HttpPost("login")] // POST: api/AccountManagement/login
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] Account account)
    {
        // Check if account details are passed
        if (account == null) return BadRequest("Account details are required.");

        // Check if email exists
        var isValidEmail = await userManager.FindByEmailAsync(account.Email);
        
        if (isValidEmail == null) return Unauthorized("Invalid Email or Password");

        // Check if password is correct
        var isValidPassword = await userManager.CheckPasswordAsync(isValidEmail, account.Password);

        if (!isValidPassword) return Unauthorized("Invalid Email or Password");

        // Get user role
        var userRoles = await userManager.GetRolesAsync(isValidEmail);

        // Create claims that will be included in the token
        // This ensures that the token contains necessary information about the user
        var authClaims = new List<Claim>
        {
            new(ClaimTypes.Name, isValidEmail.Email!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // For each role the user has, add a corresponding claim
        // This allows role-based authorization in the application
        foreach (var role in userRoles) authClaims.Add(new(ClaimTypes.Role, role));

        // Generate a symmetric security key from the secret key specified in configuration
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]!));

        // Generate a JWT token that includes the claims and is signed with the security key
        var token = new JwtSecurityToken(
            issuer: _configuration["JWT:ValidIssuer"],
            audience: _configuration["JWT:ValidAudience"],
            expires: DateTime.Now.AddHours(1),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );

        var response = new LoginResponse
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Expiration = token.ValidTo
        };

        // Return the generated token and its expiration time to the client
        return Ok(response);
    }

    // Logout endpoint to remove users authentication
    // Handles user logout by blacklisting the JWT token.
    // This prevents the token from being used for subsequent requests.
    [HttpPost("logout")] // POST: /api/AccountManager/logout
    [Authorize]
    public IActionResult Logout()
    {
        // Get the JTI (unique token) from the current user's claims.
        // The [Authorize] attribute of this endpoint ensures the token is valid at this point.
        var jti = User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;

        if (jti != null)
        {
            //When token is available add it to the blacklist
            blacklistService.BlacklistToken(jti);
            return Ok(new {message = "Logout successful"});
        }

        return BadRequest(new { message = "Invalid token" });
    }
}
