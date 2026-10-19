using System.Text.Json.Serialization;

namespace FlightService.Providers.Ignav
{
    internal record IgnavBookingLink(
        [property: JsonPropertyName("provider_name")] string ProviderName,
        [property: JsonPropertyName("provider_type")] string ProviderType,
        [property: JsonPropertyName("fare_name")] string FareName,
        [property: JsonPropertyName("specificity")] string Specificity,
        [property: JsonPropertyName("price")] IgnavPrice Price,
        [property: JsonPropertyName("url")] string? Url);
}