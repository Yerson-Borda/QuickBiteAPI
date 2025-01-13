using BLL.Services;
using DTO.BasketDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        [HttpGet]
        public async Task<ActionResult<List<DishBasketDto>>> GetUserBasket()
        {
            var basketItems = await _basketService.GetUserBasket();

            return Ok(basketItems ?? new List<DishBasketDto>());
        }

        /// <summary>
        /// Add dish to cart
        /// </summary>
        [HttpPost("dish/{dishId}")]
        public async Task<ActionResult> AddDishToBasket(Guid dishId)
        {
            await _basketService.AddDishToBasket(dishId);
            return Ok();
        }

        /// <summary>
        /// Decrease the number of dishes in the cart (if increase = true), or remove the dish completely (increase = false)
        /// </summary>
        [HttpDelete("dish/{dishId}")]
        public async Task<ActionResult> UpdateDishInBasket(Guid dishId, [FromQuery] bool increase = false)
        {
            await _basketService.UpdateDishInBasket(dishId, increase);
            return Ok();
        }

    }
}