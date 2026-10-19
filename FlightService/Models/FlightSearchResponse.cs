namespace FlightService.Models
{
    public record FlightSearchResponse(string Origin, string Destination,
        DateOnly date, int Count, IReadOnlyList<FlightOffer> Offers);
}