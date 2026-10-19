using System.Text.Json.Serialization;

namespace FlightService.Providers.Ignav
{
    internal record IgnavPrice(
        [property: JsonPropertyName("amount")]decimal Amount,
        [property: JsonPropertyName("currency")]string Currency,
        [property: JsonPropertyName("status")] string Status);
}
