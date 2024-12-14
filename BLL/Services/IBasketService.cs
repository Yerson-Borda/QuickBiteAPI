using System.Security.Claims;
using DAL.Data;
using DTO.BasketDtos;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services
{
    public interface IBasketService
    {
        Task<List<DishBasketDto>> GetUserBasket();
        Task AddDishToBasket(Guid dishId);
        Task UpdateDishInBasket(Guid dishId, bool increase = false);
    }

    public class BasketService : IBasketService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BasketService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<DishBasketDto>> GetUserBasket()
        {
            var userId = GetUserIdFromClaims();
            var basketItems = await _context.Baskets
                .Include(b => b.Dish)
                .Where(b => b.User.Id == userId && b.Order == null)
                .Select(b => new DishBasketDto
                {
                    Id = b.Dish.Id,
                    Name = b.Dish.Name,
                    Price = b.Dish.Price,
                    TotalPrice = b.Dish.Price * b.Count,
                    Amount = b.Count,
                    ImageUrl = b.Dish.ImageUrl
                })
                .ToListAsync();

            return basketItems;
        }

        public async Task AddDishToBasket(Guid dishId)
        {
            var userId = GetUserIdFromClaims();

            var user = await _context.Users.FindAsync(userId);
            if (user == null) throw new Exception("User not found");

            var dish = await _context.Dishes.FindAsync(dishId);
            if (dish == null) throw new Exception("Dish not found");

            var basketItem = await _context.Baskets
                .FirstOrDefaultAsync(b => b.User.Id == userId && b.Dish.Id == dishId && b.Order == null);

            if (basketItem == null)
            {
                basketItem = new Basket
                {
                    Id = Guid.NewGuid(),
                    User = user,
                    Dish = dish,
                    Count = 1
                };
                _context.Baskets.Add(basketItem);
            }
            else
            {
                basketItem.Count++;
            }

            await _context.SaveChangesAsync();
        }

        public async Task UpdateDishInBasket(Guid dishId, bool increase = false)
        {
            var userId = GetUserIdFromClaims();

            var basketItem = await _context.Baskets
                .FirstOrDefaultAsync(b => b.User.Id == userId && b.Dish.Id == dishId && b.Order == null);

            if (basketItem == null) throw new Exception("Basket item not found");

            if (increase)
            {
                basketItem.Count++;
            }
            else
            {
                if (basketItem.Count > 1)
                {
                    basketItem.Count--;
                }
                else
                {
                    _context.Baskets.Remove(basketItem);
                }
            }

            await _context.SaveChangesAsync();
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
