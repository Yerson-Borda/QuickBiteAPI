using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DTO.OrderDtos
{
    public class OrderCreateDto
    {
        [JsonPropertyName("deliveryTime")]
        [Required]
        public DateTime DeliveryTime { get; set; }

        [JsonPropertyName("address")]
        [Required]
        public string Address { get; set; } = string.Empty;
    }
}
