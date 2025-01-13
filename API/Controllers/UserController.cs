using BLL.Services;
using DTO.UserDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
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

        /// <summary>
        /// Register new user
        /// </summary>
        [SwaggerResponse(StatusCodes.Status200OK)]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
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

        /// <summary>
        /// Log in to the system
        /// </summary>
        [SwaggerResponse(StatusCodes.Status200OK)]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
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

        /// <summary>
        /// Log out user from the system
        /// </summary>
        [SwaggerResponse(StatusCodes.Status200OK)]
        [SwaggerResponse(StatusCodes.Status401Unauthorized)]
        [SwaggerResponse(StatusCodes.Status403Forbidden)]
        [SwaggerResponse(StatusCodes.Status404NotFound)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
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

        /// <summary>
        /// Get user profile
        /// </summary>
        // Use the token generated in this case by login to go to profile
        [Authorize]
        [SwaggerResponse(StatusCodes.Status200OK)]
        [SwaggerResponse(StatusCodes.Status401Unauthorized)]
        [SwaggerResponse(StatusCodes.Status403Forbidden)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        [HttpGet("profile")]
        public async Task<IActionResult> Profile()
        {
            var emailClaim = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);
            return Ok(await _usersService.GetProfile(emailClaim.Value));
        }

        /// <summary>
        /// Edit user profile
        /// </summary>
        [Authorize]
        [SwaggerResponse(StatusCodes.Status200OK)]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status401Unauthorized)]
        [SwaggerResponse(StatusCodes.Status403Forbidden)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] EditProfileDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
            {
                return Unauthorized();
            }

            try
            {
                await _usersService.UpdateProfile(email, model);
                return Ok();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
