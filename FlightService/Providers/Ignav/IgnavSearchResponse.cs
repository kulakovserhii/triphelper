using System.Text.Json.Serialization;

namespace FlightService.Providers.Ignav
{
    internal record IgnavSearchResponse([property: JsonPropertyName("itineraries")] List<IgnavItinerary>? Itineraries);
}
