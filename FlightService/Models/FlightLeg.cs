namespace FlightService.Models
{
    public record FlightLeg(string Carrier, int DurationMinutes,
        IReadOnlyList<FlightSegment> Segments)
    {
        public int Stops => Math.Max(0, Segments.Count - 1);
    }
}