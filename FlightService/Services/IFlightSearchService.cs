using FlightService.Models;
using FlightService.Models.FlightModels;

namespace FlightService.Services
{
    public interface IFlightSearchService
    {
        Task<IReadOnlyList<FlightOffer>> SearchAsync(FlightSearchQuery query, CancellationToken ct = default);
        Task<IReadOnlyList<BookingOption>> GetBookingOptionsAsync(string offerId, CancellationToken ct = default);
    }
}
