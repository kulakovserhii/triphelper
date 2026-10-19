using System.Text.Json.Serialization;

namespace FlightService.Providers.Ignav
{
    internal record IgnavBookingOption(
        [property: JsonPropertyName("legs")] List<string>? Legs,
        [property: JsonPropertyName("links")] List<IgnavBookingLink>? Links);
}
