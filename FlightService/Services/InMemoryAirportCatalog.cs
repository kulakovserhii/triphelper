using FlightService.Models;
using System.Globalization;
using System.Text;
using System.Text.Json;

namespace FlightService.Services
{
    public sealed class InMemoryAirportCatalog : IAirportCatalog
    {
        private sealed record Entry(Airport Airport, string City, string Name);
        private readonly Dictionary<string, Airport> _byIata;
        private readonly Entry[] _entries;
        public InMemoryAirportCatalog(IEnumerable<Airport> airports)
        {
            var list = airports.ToArray();
            _byIata = list.ToDictionary(a => a.Iata, StringComparer.OrdinalIgnoreCase);
            _entries = list.Select(a => new Entry(a, Normalize(a.City), Normalize(a.Name))).ToArray();
        }

        public static InMemoryAirportCatalog LoadFromFile(string filePath)
        {
            using var stream = File.OpenRead(filePath);
            var airports = JsonSerializer.Deserialize<List<Airport>>(stream, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? throw new InvalidOperationException("airports.json is empty");
            return new InMemoryAirportCatalog(airports);
        }

        public Airport? GetByIata(string code) => _byIata.GetValueOrDefault(code.Trim());
        private string Normalize(string s)
        {
            var decomposed = s.Trim().ToLowerInvariant().Replace('ł', 'l').Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder(decomposed.Length);
            foreach (var ch in decomposed)
                if (CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark)
                    sb.Append(ch);
            return sb.ToString().Normalize(NormalizationForm.FormC);
        }

        public IReadOnlyList<Airport> Search(string query, int limit = 10)
        {
            var q = Normalize(query);
            if (q.Length < 2) return [];

            return _entries
                .Select(e => (e.Airport, Score: Score(e, q)))
                .Where(x => x.Score > 0)
                .OrderByDescending(x => x.Score)
                .ThenBy(x => x.Airport.Type == "large" ? 0 : x.Airport.Type == "medium" ? 1 : 2)
                .ThenBy(x => x.Airport.Iata)
                .Take(limit)
                .Select(x => x.Airport)
                .ToList();
        }
        private static int Score(Entry e, string q)
        {
            if (e.Airport.Iata.Equals(q, StringComparison.OrdinalIgnoreCase)) return 100;
            if (e.City.StartsWith(q)) return 80;
            if (e.Name.StartsWith(q)) return 60;
            if (e.City.Contains(q) || e.Name.Contains(q)) return 40;
            return 0;
        }
    }
}
