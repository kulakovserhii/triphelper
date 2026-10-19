using System.Text.Json.Serialization;

namespace FlightService.Providers.Ignav
{
    internal record IgnavLeg(
        [property: JsonPropertyName("carrier")]string Carrier,
        [property: JsonPropertyName("duration_minutes")]int DurationInMinutes,
        [property: JsonPropertyName("segments")]List<IgnavSegment> Segments);
}
