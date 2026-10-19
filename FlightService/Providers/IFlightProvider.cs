using FlightService.Models;
using FlightService.Models.FlightModels;

namespace FlightService.Providers
{
    public interface IFlightProvider
    {
        Task<IReadOnlyList<FlightOffer>> SearchOneWayAsync(FlightSearchQuery query, CancellationToken ct = default);
        Task<IReadOnlyList<BookingOption>> GetBookingOptionsAsync(string offerId, CancellationToken ct = default);
    }
}
