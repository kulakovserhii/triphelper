namespace FlightService.Models
{
    public record FlightSearchQuery(string Origin, string Destination, DateOnly DepartureDate);
}