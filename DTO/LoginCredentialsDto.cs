using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DTO
{
    public class LoginCredentialsDto
    {
        [JsonPropertyName("email")]
        [Required]
        public string Email { get; set; }
        [JsonPropertyName("password")]
        [Required]
        public string Password { get; set; }
    }
}
