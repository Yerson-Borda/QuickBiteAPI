using DTO.BasketDtos;
using DTO.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DTO.OrderDtos
{
    public class OrderDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        [JsonPropertyName("deliveryTime")]
        [Required]
        public DateTime DeliveryTime { get; set; }
        [JsonPropertyName("orderTime")]
        [Required]
        public DateTime OrderTime { get; set; }
        [JsonPropertyName("status")]
        [Required]
        public Status Status { get; set; } = Status.Delivered;
        [JsonPropertyName("price")]
        [Required]
        public decimal Price { get; set; }
        [JsonPropertyName("dishes")]
        [Required]
        public ICollection<DishBasketDto> Dishes { get; set; }
        [JsonPropertyName("address")]
        [Required]
        public string Address { get; set; } = string.Empty;
    }
}
