using System.Text.Json.Serialization;

namespace DTO.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Sorting
    {
        NameAsc,
        NameDesc,
        PriceAsc,
        PriceDesc,
        RatingAsc,
        RatingDesc,
    }
}
