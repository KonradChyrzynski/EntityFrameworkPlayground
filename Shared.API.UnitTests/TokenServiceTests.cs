using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;
using Shared.API.Models;
using Shared.API.Tokens;

namespace Shared.API.Tests.Tokens;

public class TokenServiceTests
{
    private readonly TokenOptions _mockTokenOptions;
    private readonly TokenService _sut;

    public TokenServiceTests()
    {
        _mockTokenOptions = new TokenOptions
        {
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            SigningKey = "super_secret_signing_key_that_is_very_long_at_least_32_bytes!",
        };

        var optionsWrapper = Options.Create(_mockTokenOptions);

        _sut = new TokenService(optionsWrapper);
    }

    [Fact]
    public void GenerateBearerToken_ShouldReturnValidJwtString()
    {
        // Act
        var tokenString = _sut.GenerateBearerToken();

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(tokenString));

        var handler = new JwtSecurityTokenHandler();

        Assert.True(handler.CanReadToken(tokenString));
    }

    [Fact]
    public void GenerateBearerToken_ShouldContainCorrectClaimsAndConfiguration()
    {
        // Act
        var tokenString = _sut.GenerateBearerToken();

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(tokenString);

        Assert.Multiple(() =>
        {
            Assert.Equal(_mockTokenOptions.Issuer, jwtToken.Issuer);
            Assert.Contains(_mockTokenOptions.Audience, jwtToken.Audiences);
        });

        var idClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "nameid");
        var emailClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "email");
        var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "role");

        Assert.Multiple(() =>
        {
            Assert.NotNull(idClaim);
            Assert.Equal("1", idClaim.Value);

            Assert.NotNull(emailClaim);
            Assert.Equal("user@example.com", emailClaim.Value);

            Assert.NotNull(roleClaim);
            Assert.Equal("User", roleClaim.Value);
        });
    }

    //TODO: Test expiration date after refactor
}
