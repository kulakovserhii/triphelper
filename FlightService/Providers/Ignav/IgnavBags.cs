using System.Text.Json.Serialization;

namespace FlightService.Providers.Ignav
{
    internal record IgnavBags(
        [property: JsonPropertyName("carry_on")] int? CarryOn,
        [property: JsonPropertyName("checked")]int? Checked);
}
