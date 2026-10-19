namespace FlightService.Models.FlightModels
{
    public record BookingLinksResponse(string OfferId, IReadOnlyList<BookingOption> Options);
}