using BLL.Services;
using DTO.BasketDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BasketController : ControllerBase
    {
        private readonly IBasketService _basketService;

        public BasketController(IBasketService basketService)
        {
            _basketService = basketService;
        }

        /// <summary>
        /// Gets the user's basket
        /// </summary>
        [SwaggerResponse(StatusCodes.Status200OK, "Successfully retrieved the user's basket.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "User is not authenticated.")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "User is not authorized to access this resource.")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "An internal server error occurred.")]
        [HttpGet]
        public async Task<ActionResult<List<DishBasketDto>>> GetUserBasket()
        {
            var basketItems = await _basketService.GetUserBasket();
            return Ok(basketItems ?? new List<DishBasketDto>());
        }

        /// <summary>
        /// Add dish to basket
        /// </summary>
        [SwaggerResponse(StatusCodes.Status200OK, "Dish successfully added to the basket.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "User is not authenticated.")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "User is not authorized to access this resource.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Dish not found.")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "An internal server error occurred.")]
        [HttpPost("dish/{dishId}")]
        public async Task<ActionResult> AddDishToBasket(Guid dishId)
        {
            await _basketService.AddDishToBasket(dishId);
            return Ok();
        }

        /// <summary>
        /// Update dish in basket
        /// </summary>
        [SwaggerResponse(StatusCodes.Status200OK, "Dish successfully updated in the basket.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "User is not authenticated.")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "User is not authorized to access this resource.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Dish not found in the basket.")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "An internal server error occurred.")]
        [HttpDelete("dish/{dishId}")]
        public async Task<ActionResult> UpdateDishInBasket(Guid dishId, [FromQuery] bool increase = false)
        {
            await _basketService.UpdateDishInBasket(dishId, increase);
            return Ok();
        }
    }
}
