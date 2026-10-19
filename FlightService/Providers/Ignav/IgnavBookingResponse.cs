using System.Text.Json.Serialization;

namespace FlightService.Providers.Ignav
{
    internal record IgnavBookingResponse(
        [property: JsonPropertyName("booking_options")] List<IgnavBookingOption>? BookingOptions);
}
