using DAL.Data;
using DTO.Enums;
using DTO.OrderDtos;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DTO.BasketDtos;

namespace BLL.Services
{
    public interface IOrderService
    {
        Task<(bool Success, string ErrorMessage, Guid OrderId)> CreateOrder(OrderCreateDto orderCreateDto);
        Task<List<OrderInfoDto>> GetUserOrders();
        Task<(bool Success, string ErrorMessage)> ConfirmOrderDelivery(Guid id);
        Task<OrderDto> GetOrderById(Guid orderId);
    }

    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly IBasketService _basketService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrderService(ApplicationDbContext context, IBasketService basketService, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _basketService = basketService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<(bool Success, string ErrorMessage, Guid OrderId)> CreateOrder(OrderCreateDto orderCreateDto)
        {
            if (orderCreateDto.DeliveryTime <= DateTime.UtcNow.AddMinutes(60))
            {
                return (false, "Delivery time must be at least 60 minutes from now.", Guid.Empty);
            }

            var userId = GetUserIdFromClaims();
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return (false, "User not found.", Guid.Empty);
            }

            var basketItems = await _basketService.GetUserBasket();

            if (basketItems == null || !basketItems.Any())
            {
                return (false, "Basket is empty.", Guid.Empty);
            }

            var totalPrice = basketItems.Sum(item => item.TotalPrice);

            var order = new Order
            {
                Id = Guid.NewGuid(),
                DeliveryTime = orderCreateDto.DeliveryTime,
                OrderTime = DateTime.UtcNow,
                Status = Status.InProcess,
                Price = totalPrice,
                UserId = userId,
                Address = orderCreateDto.Address,
                Baskets = await _context.Baskets.Where(b => b.User.Id == userId && b.Order == null).ToListAsync()
            };

            foreach (var basketItem in order.Baskets)
            {
                basketItem.Order = order;
            }

            _context.Order.Add(order);
            await _context.SaveChangesAsync();

            return (true, null, order.Id);
        }

        public async Task<List<OrderInfoDto>> GetUserOrders()
        {
            var userId = GetUserIdFromClaims();

            var orders = await _context.Order
                .Where(o => o.UserId == userId)
                .Select(o => new OrderInfoDto
                {
                    Id = o.Id,
                    DeliveryTime = o.DeliveryTime,
                    OrderTime = o.OrderTime,
                    Status = o.Status,
                    Price = o.Price
                })
                .ToListAsync();

            return orders;
        }

        public async Task<(bool Success, string ErrorMessage)> ConfirmOrderDelivery(Guid id)
        {
            var userId = GetUserIdFromClaims();

            var order = await _context.Order.FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

            if (order == null)
            {
                return (false, "Order not found.");
            }

            if (order.Status == Status.Delivered)
            {
                return (false, "Order is already delivered.");
            }

            order.Status = Status.Delivered;
            await _context.SaveChangesAsync();

            return (true, null);
        }

        public async Task<OrderDto> GetOrderById(Guid orderId)
        {
            var userId = GetUserIdFromClaims();

            var order = await _context.Order
                .Include(o => o.Baskets)
                .ThenInclude(b => b.Dish)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

            if (order == null)
            {
                return null;
            }

            var orderDto = new OrderDto
            {
                Id = order.Id,
                DeliveryTime = order.DeliveryTime,
                OrderTime = order.OrderTime,
                Status = order.Status,
                Price = order.Price,
                Address = order.Address,
                Dishes = order.Baskets.Select(b => new DishBasketDto
                {
                    Id = b.Dish.Id,
                    Name = b.Dish.Name,
                    Price = b.Dish.Price,
                    TotalPrice = b.Dish.Price * b.Count,
                    Amount = b.Count,
                    ImageUrl = b.Dish.ImageUrl
                }).ToList()
            };

            return orderDto;
        }

        private Guid GetUserIdFromClaims()
        {
            var userIdString = _httpContextAccessor.HttpContext.User.FindFirstValue("Id");
            if (string.IsNullOrEmpty(userIdString))
                throw new UnauthorizedAccessException("User ID claim not found");

            return Guid.Parse(userIdString);
        }
    }
}
