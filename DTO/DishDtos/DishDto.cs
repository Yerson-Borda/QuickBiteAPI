using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using DTO.Enums;

namespace DTO.DishDtos
{
    public class DishDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        [JsonPropertyName("name")]
        [Required]
        public string Name { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("price")]
        [Required]
        public decimal Price { get; set; }
        [JsonPropertyName("image")]
        public string ImageUrl { get; set; }
        [JsonPropertyName("vegetarian")]
        public bool IsVegetarian { get; set; }
        [JsonPropertyName("rating")]
        public ICollection<RatingDto> Rating { get; set; }
        [JsonPropertyName("category")]
        public Category Category { get; set; }
    }
}
