using DTO.Enums;

namespace DAL.Data
{
    public class Dish
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public bool IsVegetarian { get; set; }
        // public double Rating { get; set; }
        public Category Category { get; set; }


        public ICollection<Rating>? RatingList { get; set; }
        public ICollection<Basket> DishInBasket { get; set; } = new List<Basket>();
    }
}
