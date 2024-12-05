using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DTO.BasketDtos
{
    public class DishBasketDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        [JsonPropertyName("name")]
        [Required]
        public string Name { get; set; }
        [JsonPropertyName("price")]
        [Required]
        public decimal Price { get; set; }
        [JsonPropertyName("totalPrice")]
        [Required]
        public decimal TotalPrice { get; set; }
        [JsonPropertyName("amount")]
        [Required]
        public int Amount { get; set; }
        [JsonPropertyName("image")]
        [Required]
        public string ImageUrl { get; set; }
    }
}
