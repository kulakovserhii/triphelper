namespace FlightService.Validators
{
    public record SearchFlightsRequest(string Origin, string Destination, DateOnly Date);
}
