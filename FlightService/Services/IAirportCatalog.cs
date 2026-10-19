using FlightService.Models;

namespace FlightService.Services
{
    public interface IAirportCatalog
    {
        Airport? GetByIata(string code);
        IReadOnlyList<Airport> Search(string query, int limit = 10);
    }
}
