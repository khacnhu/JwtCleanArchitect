using Application.Contracts;
using Application.Dtos;
using Application.Dtos.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApiPresentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUser user;

        public UserController(IUser user)
        {
            this.user = user;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> LogUserIn(LoginDto loginDto)
        {
            var result = await user.LoginUserAsync(loginDto);
            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<ActionResult<LoginResponse>> RegisterUser(RegisterUserDto registerUserDto)
        {
            var result = await user.RegisterUserAsync(registerUserDto);
            return Ok(result);
        }

    }
}
