using System.Text.Json.Serialization;

namespace FlightService.Providers.Ignav
{
    internal record IgnavSegment(
        [property: JsonPropertyName("marketing_carrier_code")]string MarketingCarrierCode,
        [property: JsonPropertyName("flight_number")]string FlightNumber,
        [property: JsonPropertyName("operating_carrier_name")]string OperatingCarrierName,
        [property: JsonPropertyName("departure_airport")]string DepartureAirport,
        [property: JsonPropertyName("departure_time_local")] DateTime DepartureTimeLocal,
        [property: JsonPropertyName("departure_timezone")]string DepartureTimeZone,
        [property: JsonPropertyName("departure_time_utc")]DateTime DepartureTimeUtc,
        [property: JsonPropertyName("arrival_airport")]string ArrivalAirport,
        [property: JsonPropertyName("arrival_time_local")]DateTime ArrivalTimeLocal,
        [property: JsonPropertyName("arrival_timezone")]string ArrivalTimeZone,
        [property: JsonPropertyName("arrival_time_utc")]DateTime ArrivalTimeUtc,
        [property: JsonPropertyName("duration_minutes")]int DurationInMinutes,
        [property: JsonPropertyName("aircraft")]string? Aircraft);
}