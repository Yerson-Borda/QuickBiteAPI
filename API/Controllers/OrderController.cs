using BLL.Services;
using DTO.OrderDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

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
        [SwaggerResponse(StatusCodes.Status200OK)]
        [SwaggerResponse(StatusCodes.Status401Unauthorized)]
        [SwaggerResponse(StatusCodes.Status403Forbidden)]
        [SwaggerResponse(StatusCodes.Status404NotFound)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
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
        [SwaggerResponse(StatusCodes.Status200OK)]
        [SwaggerResponse(StatusCodes.Status401Unauthorized)]
        [SwaggerResponse(StatusCodes.Status403Forbidden)]
        [SwaggerResponse(StatusCodes.Status404NotFound)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        [HttpGet]
        public async Task<ActionResult<List<OrderInfoDto>>> GetUserOrders()
        {
            var orders = await _orderService.GetUserOrders();
            return Ok(orders);
        }

        /// <summary>
        /// Creating the order from dishes in basket
        /// </summary>
        [SwaggerResponse(StatusCodes.Status200OK)]
        [SwaggerResponse(StatusCodes.Status401Unauthorized)]
        [SwaggerResponse(StatusCodes.Status403Forbidden)]
        [SwaggerResponse(StatusCodes.Status404NotFound)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
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
        [SwaggerResponse(StatusCodes.Status200OK)]
        [SwaggerResponse(StatusCodes.Status401Unauthorized)]
        [SwaggerResponse(StatusCodes.Status403Forbidden)]
        [SwaggerResponse(StatusCodes.Status404NotFound)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
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
