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
    }
}
