using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using DTO.Enums;

namespace DTO
{
    public class UserCreateDto
    {
        [JsonPropertyName("fullName")]
        [Required]
        public string Name { get; set; }
        [JsonPropertyName("password")]
        [Required]
        public string Password { get; set; }
        [JsonPropertyName("email")]
        [Required]
        public string Email { get; set; }
        [JsonPropertyName("address")]
        public string? Address { get; set; }
        [JsonPropertyName("birthDate")]
        public DateTime? BirthDate { get; set; }
        [JsonPropertyName("gender")]
        [Required]
        public Gender Gender { get; set; }
        [JsonPropertyName("phoneNumber")]
        public string? PhoneNumber { get; set; }
    }
}
