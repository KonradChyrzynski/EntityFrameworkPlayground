using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.API.Services;
using Shared.API.Tokens;

namespace Shared.API.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController(AccountService accountService) : ControllerBase
    {
        private readonly AccountService _accountService = accountService;

        [HttpPost("login")]
        [AllowAnonymous]
        public IActionResult LoginUser([FromBody] UserLoginRequestDto loginData)
        {
            var result = _accountService.LoginUser(loginData);
            if (result == null)
                return Unauthorized();
            else
                return Ok(result);
        }
    }
}
