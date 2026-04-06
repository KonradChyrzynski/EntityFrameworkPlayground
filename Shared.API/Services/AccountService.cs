using Shared.API.Tokens;

namespace Shared.API.Services;

public class AccountService(TokenService tokenService)
{
    private readonly TokenService _tokenService = tokenService;

    public TokenInfoDto? LoginUser(UserLoginRequestDto loginData)
    {
        if (loginData.UserName == "admin" && loginData.Password == "admin")
        {
            var result = new TokenInfoDto
            {
                AccessToken = _tokenService.GenerateBearerToken(),
                RefreshToken = _tokenService.GenerateRefreshToken(),
            };

            return result;
        }
        else
            return null;
    }
}
