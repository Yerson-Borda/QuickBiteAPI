using BLL.Services;
using DTO.Enums;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DishController : ControllerBase
    {
        private readonly IDishService _dishService;

        public DishController(IDishService dishService)
        {
            _dishService = dishService;
        }

        [HttpGet]
        public async Task<IActionResult> GetDishes([FromQuery] List<Category> categories, [FromQuery] bool? vegetarian, [FromQuery] Sorting? sorting, [FromQuery] int page = 1)
        {
            try
            {
                var response = await _dishService.GetDishes(categories, vegetarian, sorting, page);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDishById(Guid id)
        {
            try
            {
                var dish = await _dishService.GetDishById(id);
                if (dish == null)
                {
                    return NotFound();
                }
                return Ok(dish);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("{id}/rating/check")]
        public async Task<IActionResult> CanRateDish(Guid id)
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
                if (userId == null) return Unauthorized();

                var canRate = await _dishService.CanRateDish(id, Guid.Parse(userId));
                return Ok(canRate);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("{id}/rating")]
        public async Task<IActionResult> SetRating([FromRoute] Guid id, [FromQuery] int ratingScore)
        {
            try
            {
                if (ratingScore < 1 || ratingScore > 10)
                {
                    return BadRequest("Rating must be between 1 and 10.");
                }

                var userId = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User is not authenticated.");
                }

                await _dishService.SetRating(id, Guid.Parse(userId), ratingScore);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
