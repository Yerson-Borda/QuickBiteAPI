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

        /// <summary>
        /// Gets information about concrete order
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetOrderById(Guid id)
        {
            var order = await _orderService.GetOrderById(id);
            if (order == null)
            {
                return NotFound();
            }
            return Ok(order);
        }

        /// <summary>
        /// Get a list of orders
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<OrderInfoDto>>> GetUserOrders()
        {
            var orders = await _orderService.GetUserOrders();
            return Ok(orders);
        }

        /// <summary>
        /// Creating the order from dishes in basket
        /// </summary>
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

        /// <summary>
        /// Confirm order delivery
        /// </summary>
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
