using DAL.Data;
using DTO.DishDtos;
using DTO.Enums;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services
{
    public interface IDishService
    {
        Task<PagedResponse<DishDto>> GetDishes(List<Category> categories, bool? vegetarian, Sorting? sorting, int page);
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
            int pageSize = 10; // Define a default page size

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
                    query = query.OrderBy(d => d.Rating);
                    break;
                case Sorting.RatingDesc:
                    query = query.OrderByDescending(d => d.Rating);
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
                    Name = d.Name,
                    Description = d.Description,
                    Price = d.Price,
                    ImageUrl = d.ImageUrl,
                    IsVegetarian = d.IsVegetarian,
                    Rating = d.Rating,
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
    }
}