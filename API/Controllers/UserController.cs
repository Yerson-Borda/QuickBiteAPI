using BLL.Services;
using DTO;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUsersService _usersService;
        public UserController(IUsersService usersService)
        {
            _usersService = usersService;
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
    }
}
