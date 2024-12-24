using BLL.Services;
using DTO.OrderDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<ActionResult> CreateOrder([FromBody] OrderCreateDto orderCreateDto)
        {
            var result = await _orderService.CreateOrder(orderCreateDto);
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok(new { result.OrderId });
        }

        [HttpGet]
        public async Task<ActionResult<List<OrderInfoDto>>> GetUserOrders()
        {
            var orders = await _orderService.GetUserOrders();
            return Ok(orders);
        }

        [HttpPost("{id}/status")]
        public async Task<ActionResult> ConfirmOrderDelivery(Guid id)
        {
            var result = await _orderService.ConfirmOrderDelivery(id);
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok();
        }
    }
}
