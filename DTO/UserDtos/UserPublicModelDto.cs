using DTO.Enums;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace DTO.UserDtos
{
    public class UserPublicModelDto
    {
        [JsonPropertyName("fullName")]
        public string Name { get; set; }
        [JsonPropertyName("birthDate")]
        public DateOnly BirthDate { get; set; }
        [JsonPropertyName("gender")]
        public Gender Gender { get; set; }
        [JsonPropertyName("address")]
        public string Address { get; set; }
        [JsonPropertyName("email")]
        public string Email { get; set; }
        [JsonPropertyName("phoneNumber")]
        public string PhoneNumber { get; set; }
    }
}
