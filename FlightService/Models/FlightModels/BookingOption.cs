using System.Text.Json.Serialization;

namespace FlightService.Models.FlightModels
{
    public record BookingOption(
        [property: JsonPropertyName("legs")]IReadOnlyList<string> Legs,
        [property: JsonPropertyName("links")]IReadOnlyList<BookingLink> Links);
}
