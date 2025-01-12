using DAL.Data;
using DTO.DishDtos;
using DTO.Enums;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services
{
    public interface IDishService
    {
        Task<PagedResponse<DishDto>> GetDishes(List<Category> categories, bool? vegetarian, Sorting? sorting, int page);
        Task<DishDto> GetDishById(Guid id);

        Task<bool> CanRateDish(Guid dishId, Guid userId);
        Task SetRating(Guid dishId, Guid userId, int ratingScore);
    }

    public class DishService : IDishService
    {
        private readonly ApplicationDbContext _context;

        public DishService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResponse<DishDto>> GetDishes(List<Category> categories, bool? vegetarian, Sorting? sorting, int page)
        {
            int pageSize = 10;

            var query = _context.Dishes.AsQueryable();

            if (categories != null && categories.Any())
            {
                query = query.Where(d => categories.Contains(d.Category));
            }

            if (vegetarian.HasValue)
            {
                query = query.Where(d => d.IsVegetarian == vegetarian.Value);
            }

            switch (sorting)
            {
                case Sorting.PriceAsc:
                    query = query.OrderBy(d => d.Price);
                    break;
                case Sorting.PriceDesc:
                    query = query.OrderByDescending(d => d.Price);
                    break;
                case Sorting.RatingAsc:
                    query = query.OrderBy(d => d.RatingList.Average(r => r.Value));
                    break;
                case Sorting.RatingDesc:
                    query = query.OrderByDescending(d => d.RatingList.Average(r => r.Value));
                    break;
                case Sorting.NameAsc:
                    query = query.OrderBy(d => d.Name);
                    break;
                case Sorting.NameDesc:
                    query = query.OrderByDescending(d => d.Name);
                    break;
                default:
                    query = query.OrderBy(d => d.Name);
                    break;
            }

            var totalItems = await query.CountAsync();
            var dishes = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(d => new DishDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    Description = d.Description,
                    Price = d.Price,
                    ImageUrl = d.ImageUrl,
                    IsVegetarian = d.IsVegetarian,
                    Rating = d.RatingList.Select(r => new RatingDto
                    {
                        DishId = r.DishId,
                        Value = r.Value
                    }).ToList(),
                    Category = d.Category
                })
                .ToListAsync();


            var response = new PagedResponse<DishDto>
            {
                Dishes = dishes,
                Pagination = new Pagination
                {
                    Size = pageSize,
                    Count = totalItems,
                    Current = page
                }
            };

            return response;
        }

        public async Task<DishDto> GetDishById(Guid id)
        {
            var dish = await _context.Dishes
                .Include(d => d.RatingList) // Include RatingList to load related ratings
                .FirstOrDefaultAsync(d => d.Id == id);

            if (dish == null)
            {
                return null;
            }

            return new DishDto
            {
                Id = dish.Id,
                Name = dish.Name,
                Description = dish.Description,
                Price = dish.Price,
                ImageUrl = dish.ImageUrl,
                IsVegetarian = dish.IsVegetarian,
                Rating = dish.RatingList.Select(r => new RatingDto
                {
                    DishId = r.DishId,
                    Value = r.Value
                }).ToList(),
                Category = dish.Category
            };
        }

        public async Task<bool> CanRateDish(Guid dishId, Guid userId)
        {
            var dishExists = await _context.Dishes.AnyAsync(d => d.Id == dishId);
            if (!dishExists) throw new KeyNotFoundException("Dish not found");

            var hasOrdered = await _context.Order
                .AnyAsync(o => o.UserId == userId && o.Baskets.Any(b => b.Dish.Id == dishId));

            return hasOrdered;
        }

        public async Task SetRating(Guid dishId, Guid userId, int ratingScore)
        {
            if (ratingScore < 1 || ratingScore > 10) throw new ArgumentException("Rating must be between 1 and 10");

            if (!await CanRateDish(dishId, userId))
                throw new InvalidOperationException("User is not eligible to rate this dish");

            // Add rating to database
            var rating = new Rating
            {
                Id = Guid.NewGuid(),
                DishId = dishId,
                UserId = userId,
                Value = ratingScore
            };

            _context.Rating.Add(rating);
            await _context.SaveChangesAsync();
        }
    }
}