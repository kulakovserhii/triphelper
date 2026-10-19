using FlightService.Models;
using FlightService.Models.FlightModels;
using FlightService.Providers;

namespace FlightService.Services
{
    public class FLightSearchService(IFlightProvider provider) : IFlightSearchService
    {
        public Task<IReadOnlyList<BookingOption>> GetBookingOptionsAsync(string offerId, CancellationToken ct = default) =>
            provider.GetBookingOptionsAsync(offerId, ct);

        public async Task<IReadOnlyList<FlightOffer>> SearchAsync(FlightSearchQuery query, CancellationToken ct = default)
        {
            var offers = await provider.SearchOneWayAsync(query, ct);
            return offers.OrderBy(o => o.Price)
                .ThenBy(o => o.Outbound.DurationMinutes)
                .ThenBy(o => o.Outbound.Segments.FirstOrDefault()?.DepartureUtc)
                .ToList();
        }
    }
}