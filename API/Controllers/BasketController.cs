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

        [HttpGet]
        public async Task<ActionResult<List<DishBasketDto>>> GetUserBasket()
        {
            var basketItems = await _basketService.GetUserBasket();

            return Ok(basketItems ?? new List<DishBasketDto>());
        }

        [HttpPost("dish/{dishId}")]
        public async Task<ActionResult> AddDishToBasket(Guid dishId)
        {
            await _basketService.AddDishToBasket(dishId);
            return Ok();
        }

        [HttpDelete("dish/{dishId}")]
        public async Task<ActionResult> UpdateDishInBasket(Guid dishId, [FromQuery] bool increase = false)
        {
            await _basketService.UpdateDishInBasket(dishId, increase);
            return Ok();
        }

    }
}