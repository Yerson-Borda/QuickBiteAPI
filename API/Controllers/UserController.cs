using BLL.Services;
using DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUsersService _usersService;
        private readonly ITokenService _tokenService;
        public UserController(IUsersService usersService, ITokenService tokenService)
        {
            _usersService = usersService;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserCreateDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                await _usersService.Register(model);
            }
            catch (ArgumentException ex)
            {
                return BadRequest("User with same email already exists!");
            }
            catch (Exception ex)
            {
                return Problem("Something happened during users registration");
            }
            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCredentialsDto model)
        {
            try
            {
                return Ok(await _usersService.Login(model));
            }
            catch (ArgumentException ex)
            {
                return BadRequest("Login or password is incorrect");
            }
            catch (Exception ex)
            {
                return Problem();
            }
        }

        // Use the token generated in this case by login to go to profile
        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> Profile()
        {
            var emailClaim = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);
            return Ok(await _usersService.GetProfile(emailClaim.Value));
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(x => x.Type == "Id");
                await _tokenService.Logout(Guid.Parse(userIdClaim.Value));
                return Ok();
            }
            catch (Exception ex)
            {
                return Problem();
            }
        }
    }
}
