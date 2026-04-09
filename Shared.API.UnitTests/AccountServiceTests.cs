using Shared.API.Services;
using Shared.API.Tokens;

namespace Shared.API.UnitTests;

public class AccountServiceTests
{
    private readonly TokenService _mockTokenService;
    private readonly AccountService _sut;

    public AccountServiceTests()
    {
        var tokenOptions = Microsoft.Extensions.Options.Options.Create(
            new Models.TokenOptions
            {
                Issuer = "TestIssuer",
                Audience = "TestAudience",
                SigningKey = "super_secret_signing_key_that_is_very_long_at_least_32_bytes!",
            }
        );

        _mockTokenService = new TokenService(tokenOptions);
        _sut = new AccountService(_mockTokenService);
    }

    [Fact]
    public void LoginUser_WithValidCredentials_ReturnsTokenInfoDto()
    {
        var loginData = new UserLoginRequestDto { UserName = "admin", Password = "admin" };

        var result = _sut.LoginUser(loginData);

        Assert.NotNull(result);
        Assert.NotNull(result.AccessToken);
        Assert.NotNull(result.RefreshToken);
    }

    [Fact]
    public void LoginUser_WithInvalidUsername_ReturnsNull()
    {
        var loginData = new UserLoginRequestDto { UserName = "wronguser", Password = "admin" };

        var result = _sut.LoginUser(loginData);

        Assert.Null(result);
    }

    [Fact]
    public void LoginUser_WithInvalidPassword_ReturnsNull()
    {
        var loginData = new UserLoginRequestDto { UserName = "admin", Password = "wrongpassword" };

        var result = _sut.LoginUser(loginData);

        Assert.Null(result);
    }

    [Fact]
    public void LoginUser_WithEmptyCredentials_ReturnsNull()
    {
        var loginData = new UserLoginRequestDto { UserName = "", Password = "" };

        var result = _sut.LoginUser(loginData);

        Assert.Null(result);
    }

    [Fact]
    public void LoginUser_WithNullCredentials_ThrowsNullReferenceException()
    {
        Assert.Throws<NullReferenceException>(() => _sut.LoginUser(null!));
    }
}
