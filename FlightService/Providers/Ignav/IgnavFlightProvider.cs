using FlightService.Models;
using FlightService.Models.FlightModels;
using System.Net;
using System.Text.Json;

namespace FlightService.Providers.Ignav
{
    internal sealed class IgnavFlightProvider(HttpClient http, ILogger<IgnavFlightProvider> logger) : IFlightProvider
    {
        private static readonly JsonSerializerOptions Json = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        };
        public async Task<IReadOnlyList<FlightOffer>> SearchOneWayAsync(FlightSearchQuery query, CancellationToken ct = default)
        {
            var body = new
            {
                origin = query.Origin,
                destination = query.Destination,
                departure_date = query.DepartureDate.ToString("yyyy-MM-dd"),
            };
            var dto = await PostAsync<IgnavSearchResponse>("/api/fares/one-way", body, ct);
            return dto?.Itineraries?.Select(MapOffer).ToList() ?? [];
        }

        private async Task<T> PostAsync<T>(string path, object body, CancellationToken ct)
        {
            HttpResponseMessage response;
            try
            {
                response = await http.PostAsJsonAsync(path, body, Json, ct);
            }
            catch(HttpRequestException ex)
            {
                logger.LogWarning(ex, "Network error while connecttng to {Ignav}", path);
                throw new FlightProviderException("Провайдер недоступний", ProviderErrorKind.Unavailable);
            }
            using (response)
            {
                if (!response.IsSuccessStatusCode)
                {
                    var text = await response.Content.ReadAsStringAsync(ct);
                    logger.LogWarning("Ignav {Path} returned {Status}: {Body}", path, (int)response.StatusCode, text.Length > 300 ? text[..300]: text);
                    throw new FlightProviderException($"Провайдер повернув {(int)response.StatusCode}", Classify(response.StatusCode), response.StatusCode);
                }
                return await response.Content.ReadFromJsonAsync<T>(Json, ct)
                    ?? throw new FlightProviderException("Пороня відповідь провайдера", ProviderErrorKind.Unavailable);
            }
        }

        private static ProviderErrorKind Classify(HttpStatusCode statusCode) => (int)statusCode switch
        {
            400 or 422 => ProviderErrorKind.InvalidRequest,
            404 => ProviderErrorKind.NotFound,
            401 or 403 => ProviderErrorKind.Unauthorized,
            429 => ProviderErrorKind.RateLimited,
            _ => ProviderErrorKind.Unavailable,
        };

        private static FlightOffer MapOffer(IgnavItinerary i) => new(
            ProviderOfferId: i.IgnavId,
            Price: i.Price.Amount,
            Currency: i.Price.Currency,
            PriceStatus: i.Price.Status,
            CabinClass: i.CabinClass,
            RequiresSelfTransfer: i.RequiresSelfTransfer,
            Bags: i.Bags is null ? null : new BagAllowance(i.Bags.CarryOn, i.Bags.Checked),
            Outbound: new FlightLeg(
                i.Outbound.Carrier,
                i.Outbound.DurationInMinutes,
                i.Outbound.Segments.Select(s => new FlightSegment(
                    s.MarketingCarrierCode, s.FlightNumber, s.OperatingCarrierName,
                    s.DepartureAirport, s.DepartureTimeLocal, s.DepartureTimeZone,
                    s.DepartureTimeUtc, s.ArrivalAirport, s.ArrivalTimeLocal, s.ArrivalTimeZone,
                    s.ArrivalTimeUtc, s.DurationInMinutes, s.Aircraft)).ToList()));

       private static BookingLink? MapLink(IgnavBookingLink l)
        {
            var url = l.Url?.Trim();
            if (string.IsNullOrEmpty(url)) return null;
            if (!url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                !url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                url = "https://" + url;
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri)) return null;
            return new BookingLink(l.ProviderName, l.ProviderType, l.FareName, l.Specificity,
                l.Price?.Amount, l.Price?.Currency, uri.AbsoluteUri);
        }

        public async Task<IReadOnlyList<BookingOption>> GetBookingOptionsAsync(string offerId, CancellationToken ct = default)
        {
            var dto = await PostAsync<IgnavBookingResponse>("/api/fares/booking-links", new { ignav_id = offerId }, ct);
            var raw = dto.BookingOptions ?? [];
            var result = raw.Select(o => new BookingOption(o.Legs ?? [], (o.Links ?? [])
                    .Select(MapLink)
                    .OfType<BookingLink>()
                    .OrderBy(l => l.Specificity == "exact_flight" ? 0 : 1)
                    .ThenBy(l => l.Price ?? decimal.MaxValue)
                    .ToList()))
                .Where(o => o.Links.Count > 0)
                .ToList();
            if (result.Count == 0)
                logger.LogWarning("booking-links {OfferId}: опцій від Ignav {Raw}, посилань {Links}, після мапінгу {Mapped}",
                    offerId, raw.Count, raw.Sum(o => o.Links?.Count ?? 0), result.Count);

            return result;
        }
    }
}