using System.Text.Json.Serialization;

namespace DTO.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Category
    {
        Wok,
        Pizza,
        Soup,
        Dessert,
        Drink
    }
}
