namespace FlightService.Models
{
    public record FlightSegment(
        string CarrierCode,
        string FlightNumber,
        string OperatingCarrier,
        string DepartureAirport,
        DateTime DepartureLocal,
        string DepartureTimeZone,
        DateTime DepartureUtc,
        string ArrivalAirport,
        DateTime ArrivalLocal,
        string ArrivalTimeZone,
        DateTime ArrivalUtc,
        int DurationMinutes,
        string? Aircraft);
}
