using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DTO
{
    public class EditProfileDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("birthDate")]
        public DateTime BirthDate { get; set; }
    }
}
