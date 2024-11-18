using System.Text.Json.Serialization;
using DTO.Enums;

namespace DTO.UserDtos
{
    public class EditProfileDto
    {
        [JsonPropertyName("fullName")]
        public string Name { get; set; }
        [JsonPropertyName("birthDate")]
        public DateTime BirthDate { get; set; }
        [JsonPropertyName("gender")]
        public Gender Gender { get; set; }
        [JsonPropertyName("address")]
        public string Address { get; set; }
        [JsonPropertyName("phoneNumber")]
        public string PhoneNumber { get; set; }
    }
}
