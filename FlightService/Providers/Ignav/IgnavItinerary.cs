using System.Text.Json.Serialization;

namespace FlightService.Providers.Ignav
{
    internal record IgnavItinerary(
        [property: JsonPropertyName("price")]IgnavPrice Price, 
        [property: JsonPropertyName("outbound")]IgnavLeg Outbound,
        [property: JsonPropertyName("cabin_class")]string CabinClass,
        [property: JsonPropertyName("bags")]IgnavBags? Bags,
        [property: JsonPropertyName("requires_self_transfer")] bool RequiresSelfTransfer,
        [property: JsonPropertyName("ignav_id")] string IgnavId);
}