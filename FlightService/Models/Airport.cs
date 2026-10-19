namespace FlightService.Models
{
    public record Airport(string Iata, string Name, string City, string Country, string Type, double Lat, double Lon);
}
