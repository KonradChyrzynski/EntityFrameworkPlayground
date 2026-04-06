using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shared.API.Models;

namespace Shared.API.Tokens;

public class TokenService(IOptions<TokenOptions> tokenOptions)
{
    private readonly TokenOptions _tokenOptions = tokenOptions.Value;

    public string GenerateBearerToken()
    {
        var expiry = DateTimeOffset.Now.AddMinutes(15);
        var userClaims = GetClaimsForUser(1);
        return CreateToken(expiry, userClaims);
    }

    public string GenerateRefreshToken()
    {
        var expiry = DateTimeOffset.Now.AddDays(30);
        var userClaims = GetClaimsForUser(1);
        return CreateToken(expiry, userClaims);
    }

    private string CreateToken(DateTimeOffset expiryDate, IEnumerable<Claim> claims)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_tokenOptions.SigningKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var securityToken = new JwtSecurityToken(
            issuer: _tokenOptions.Issuer,
            audience: _tokenOptions.Audience,
            claims: claims,
            notBefore: DateTime.Now,
            expires: expiryDate.DateTime,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(securityToken);
    }

    public string CreateRefreshToken()
    {
        var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_tokenOptions.SigningKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var expiry = DateTimeOffset.Now.AddDays(30);
        var userClaims = GetClaimsForUser(1);

        var securityToken = new JwtSecurityToken(
            issuer: _tokenOptions.Issuer,
            audience: _tokenOptions.Audience,
            claims: userClaims,
            notBefore: DateTime.Now,
            expires: expiry.DateTime,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(securityToken);
    }

    private IEnumerable<Claim> GetClaimsForUser(int userId)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Email, "user@example.com"),
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Role, "User"),
        };

        return claims;
    }
}
